using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Extensions.AzureMaps.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Blazor.Extensions.AzureMaps
{
    internal class MapService : IMapService
    {
        private const string AzureMapsScriptName = "./_content/Blazor.Extensions.AzureMaps/BE.AzureMaps.js";
        private const string AzureMapsCssName = "./_content/Blazor.Extensions.AzureMaps/bundle.css";
        private const string SetSubscriptionKeyMethod = "setSubscriptionKey";
        private const string InjectCssMethod = "injectCss";
        private const string AzureMapsClass = "BEAzureMaps";
        private const string CreateMapMethod = "createMap";
        private const string AddDrawingToolsMethod = "addDrawingTools";
        private const string ImportJSModuleMethod = "import";
        private readonly AzureMapsOptions options;
        private readonly IJSRuntime runtime;
        private IJSObjectReference azureMapsModule = default!;

        public MapService(IOptions<AzureMapsOptions> options, IJSRuntime runtime)
        {
            this.runtime = runtime;
            if (string.IsNullOrWhiteSpace(options.Value.SubscriptionKey)) throw new ArgumentNullException(nameof(options.Value.SubscriptionKey));
            this.options = options.Value;
        }

        public async Task<IMapReference> CreateMap(Guid mapId, MapOptions? mapOptions)
        {
            await this.EnsureModuleLoaded();

            var jsReference = await this.azureMapsModule
                    .InvokeAsync<IJSObjectReference>(
                        $"{AzureMapsClass}.{CreateMapMethod}",
                        mapId,
                        mapOptions
                    );
            return new MapReference(mapId, jsReference);
        }

        public async Task AddDrawingTool(IMapReference mapReference, DrawingManagerOptions? drawingManagerOptions)
        {
            await this.azureMapsModule
                .InvokeVoidAsync(
                    $"{AzureMapsClass}.{AddDrawingToolsMethod}",
                    mapReference.Map,
                    drawingManagerOptions
                );
        }

        public async Task SetCamera(MapOptions options)
        {
            await this.azureMapsModule.InvokeVoidAsync(
                $"{AzureMapsClass}.setCamera",
                options
            );
        }

        public async Task AddShape(DrawingManagerOptions opts, string id, ShapeProperties? properties)
        {
            await this.azureMapsModule.InvokeVoidAsync(
                $"{AzureMapsClass}.addShape",
                opts,id,properties
            );
        }

        public async Task ClearShapes()
        {
            await this.azureMapsModule.InvokeVoidAsync(
                $"{AzureMapsClass}.clearShapes");
        }

        public async Task ClearTiles()
        {
            await this.azureMapsModule.InvokeVoidAsync(
                $"{AzureMapsClass}.clearTiles");
        }

        public async Task<List<List<int>>> GetTiles()
        {
            return await this.azureMapsModule.InvokeAsync<List<List<int>>>(
                $"{AzureMapsClass}.getTiles");
        }

        public async Task<List<int>> GetTile(double longitude, double latitude, int zoom)
        {
            return await this.azureMapsModule.InvokeAsync<List<int>>(
                $"{AzureMapsClass}.getTile", longitude,latitude,zoom);
        }

        public async Task AddPolygon(List<List<double>> boundingList, int zoom, string datasourceId, string id, PolygonOptions properties)
        {
            await this.azureMapsModule.InvokeVoidAsync(
                $"{AzureMapsClass}.addPolygon", datasourceId,boundingList,id,properties);
        }

        public async Task AddEvent(string @event)
        {
            await this.azureMapsModule.InvokeVoidAsync(
                $"{AzureMapsClass}.addEvent", @event);
        }

        public async Task<MapMouseEvent> GetMapMouseEvent()
        {
            return await this.azureMapsModule.InvokeAsync<MapMouseEvent>(
                $"{AzureMapsClass}.getMapMouseEvent");
        }

        private async ValueTask EnsureModuleLoaded()
        {
            if (this.azureMapsModule != null) return;

            this.azureMapsModule = await this.runtime.InvokeAsync<IJSObjectReference>(
                ImportJSModuleMethod, AzureMapsScriptName);

            await this.azureMapsModule.InvokeVoidAsync($"{AzureMapsClass}.{InjectCssMethod}", AzureMapsCssName);
            await this.azureMapsModule.InvokeVoidAsync($"{AzureMapsClass}.{SetSubscriptionKeyMethod}", this.options.SubscriptionKey);
        }
    }
}