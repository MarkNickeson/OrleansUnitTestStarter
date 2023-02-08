using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePatternTests.CoHostedService
{
    public class CoHostedService : IHostedService, ISiloStatusListener
    {
        IClusterClient _client;
        ISiloStatusOracle _statusOracle;
        CancellationTokenSource cts = new CancellationTokenSource();
        MeterListener? listener = null;

        public CoHostedService(IClusterClient client, ISiloStatusOracle statusOracle) // can dependency inject here from Orleans types because container is the same
        {
            _client = client;
            _statusOracle = statusOracle;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _statusOracle.SubscribeToSiloStatusEvents(this);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (observableReaderTask != null)
            {
                if (!cts.IsCancellationRequested)
                {
                    // signal cancellation to the reader task
                    cts.Cancel();
                }
         
                // wait for the reader task to exit
                await observableReaderTask;

                if (listener!= null)
                {
                    listener.Dispose();
                }
            }

            _statusOracle.UnSubscribeFromSiloStatusEvents(this);
        }

        public void SiloStatusChangeNotification(SiloAddress updatedSilo, SiloStatus status)
        {
            if (status == SiloStatus.Active)
            {
                ActivateMeterCollection();
            }
            else if (status == SiloStatus.ShuttingDown)
            {
                if (!cts.IsCancellationRequested)
                {
                    // signal cancellation to the reader task
                    cts.Cancel();
                }
            }
        }

        bool trackingObservables = false;
        Task? observableReaderTask = null;

        void ActivateMeterCollection()
        {
            // create listener
            listener = new MeterListener();

            // hook instruments published to discover current and future measurement instruments
            listener.InstrumentPublished = OnInstrumentPublished;

            // setup required measurement callback handlers; in practise need one for every possible type. And should test
            // if possible to added to listener AFTER it has been started
            listener.SetMeasurementEventCallback<int>(OnMeasurementWritten);

            listener.Start();
        }

        void OnInstrumentPublished(Instrument instrument, MeterListener currentListener)
        {
            if (instrument.Name == "orleans-catalog-activations")
            {
                currentListener.EnableMeasurementEvents(instrument);
                if (instrument.IsObservable) trackingObservables = true;
                return;
            }

            if (instrument.Name == "orleans-catalog-activation-created")
            {
                currentListener.EnableMeasurementEvents(instrument);
                if (instrument.IsObservable) trackingObservables = true;
                return;
            }

            // check if transitioning to monitoring observables
            if (trackingObservables && observableReaderTask == null)
            {
                observableReaderTask = Task.Run(ObservableRecordingLoop);
            }
        }

        async Task ObservableRecordingLoop()
        {
            if (listener == null) return;

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    await Task.Delay(1000, cts.Token);
                    listener.RecordObservableInstruments();
                }
            }
            catch (TaskCanceledException)
            {
                // suppress exception on cancellation
            }
        }

        void OnMeasurementWritten(Instrument instrument, int measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
        {
            Debug.WriteLine($"{instrument.Name}: {measurement}");
        }
    }

    public static class CoHostedServiceExtensions
    {
        public static IHostBuilder UseCoHostedService(this IHostBuilder target)
        {
            target.ConfigureServices((collection) =>
            {
                // add intended service
                collection.AddHostedService<CoHostedService>();
            });
            return target;
        }
    }
}
