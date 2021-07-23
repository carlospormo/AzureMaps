using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Extensions.AzureMaps
{
    public interface IMapService
    {
        Task<IMapReference> CreateMap(Guid mapId, MapOptions? options);
        Task AddDrawingTool(IMapReference mapReference, DrawingManagerOptions? drawingManagerOptions);
        Task SetCamera(MapOptions options);
        Task AddShape(DrawingManagerOptions opts, string id, ShapeProperties? properties);
        Task ClearShapes();
        Task ClearTiles();
        Task<List<List<int>>> GetTiles();
        Task<List<int>> GetTile(double longitude, double latitude, int zoom);
        Task AddPolygon(List<List<double>> boundingList, int zoom, string datasourceId, string id, PolygonOptions properties);
        Task AddEvent(string @event);
        Task<MapMouseEvent> GetMapMouseEvent();
    }
}