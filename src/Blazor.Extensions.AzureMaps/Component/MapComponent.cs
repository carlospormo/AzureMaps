using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Blazor.Extensions.AzureMaps
{
    public class MapComponent : ComponentBase, IAsyncDisposable
    {
        [Inject] private ILogger<MapComponent> Logger { get; set; } = default!;
        [Inject] public IMapService MapService { get; set; } = default!;
        [Parameter] public MapOptions? Options { get; set; } = default!;
        [Parameter] public DrawingManagerOptions? DrawingManagerOptions { get; set; } = default!;
        [Parameter] public EventCallback<MapMouseEvent> OnClick { get; set; }

        protected readonly Guid MapId;
        private IMapReference map = default!;
        private IMapDrawingManager drawingManager = default!;

        public MapComponent()
        {
            this.MapId = Guid.NewGuid();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.map = await this.MapService
                    .CreateMap(this.MapId, this.Options);
                if (this.OnClick.HasDelegate)
                {
                    await this.MapService.AddEvent("click");
                }
            }
        }

        protected async Task MapClicked()
        {
            if (this.OnClick.HasDelegate)
            {
                var mapMouseEvent = await this.MapService.GetMapMouseEvent();
                await this.OnClick.InvokeAsync(mapMouseEvent);
            }
        }

        public async Task SetCamera(MapOptions cameraOptions)
        {
            await this.MapService.SetCamera(cameraOptions);
        }

        public async Task AddShape(DrawingManagerOptions opts, string id, ShapeProperties? properties)
        {
            await this.MapService.AddShape(opts, id, properties);
        }

        public async Task ClearShapes()
        {
            await this.MapService.ClearShapes();
        }

        public async Task ClearTiles()
        {
            await this.MapService.ClearTiles();
        }

        public async Task<List<List<int>>> GetTiles()
        {
            return await this.MapService.GetTiles();
        }

        public async Task<List<int>> GetTile(double longitude, double latitude, int zoom)
        {
            return await this.MapService.GetTile(longitude, latitude, zoom);
        }

        public async Task AddPolygon(List<List<double>> boundingList,int zoom, string datasourceId, string id, PolygonOptions properties)
        {
            await this.MapService.AddPolygon(boundingList, zoom, datasourceId, id, properties);
        }

        public async Task<MapMouseEvent> GetMapMouseEvent()
        {
            return await this.MapService.GetMapMouseEvent();
        }

        public ValueTask DisposeAsync()
        {
            if (this.map != null)
            {
                this.drawingManager?.DisposeAsync();
                return this.map.DisposeAsync();
            }

            return ValueTask.CompletedTask;
        }
    }
}
