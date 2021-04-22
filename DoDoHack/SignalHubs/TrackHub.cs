using DoDoHack.Data;
using DoDoHack.Models;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoDoHack.SignalHubs
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class TrackHub : Hub
    {
        private static readonly Dictionary<long, Coordinates> _couriersTracks;
        private static readonly Dictionary<long, int> _trackCountsSent;

        private readonly DodoBase _dbContext;
        private readonly ILogger<TrackHub> _logger;

        static TrackHub()
        {
            _couriersTracks = new Dictionary<long, Coordinates>();
            _trackCountsSent = new Dictionary<long, int>();
        }

        public TrackHub(DodoBase db, ILogger<TrackHub> logger)
        {
            _dbContext = db;
            _logger = logger;
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = nameof(Courier))]
        public async Task UpdateTrack(Coordinates coordinates)
        {
            long courierId = long.Parse(Context.User.FindFirst("Id").Value);

            _logger.LogInformation($"{courierId} update coordinates: {coordinates.Longitude} : {coordinates.Latitude}");

            if (!_couriersTracks.ContainsKey(courierId))
            { 
                _couriersTracks.Add(courierId, coordinates);
                if(_trackCountsSent.ContainsKey(courierId)) _trackCountsSent[courierId] = 0;
                else _trackCountsSent.Add(courierId, 0);

                CourierAction action = new CourierAction()
                {
                    ActionTime = DateTime.Now,
                    CourierId = courierId,
                    Discription = "Курьер включил отслеживание."
                };
                await _dbContext.Set<CourierAction>().AddAsync(action);
            }

            if(_trackCountsSent[courierId] % 5 == 0)
            {
                Track track = new Track()
                {
                    Latitude = coordinates.Latitude,
                    Longitude = coordinates.Longitude,
                    TrackTime = DateTime.Now,
                    CourierId = courierId
                };
                await _dbContext.Set<Track>().AddAsync(track);
                
                await _dbContext.SaveChangesAsync();
            }

            _couriersTracks[courierId] = coordinates;
            _trackCountsSent[courierId]++;
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = nameof(Courier))]
        public async Task StopTrack()
        {
            long courierId = long.Parse(Context.User.FindFirst("Id").Value);

            _couriersTracks.Remove(courierId);
            _trackCountsSent.Remove(courierId);

            CourierAction action = new CourierAction()
            {
                ActionTime = DateTime.Now,
                CourierId = courierId,
                Discription = "Курьер выключил отслеживание."
            };

            await _dbContext.Set<CourierAction>().AddAsync(action);
            await _dbContext.SaveChangesAsync();
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = nameof(Admin))]
        public async Task GetTrack(long courierId)
        {
            if (_couriersTracks.ContainsKey(courierId))
                await Clients.Caller.SendAsync("GetTrack", courierId, _couriersTracks[courierId]);
            else
                await Clients.Caller.SendAsync("NoTracking", courierId);
        }

        public async Task GetTracksByDate(long courierId, DateTime byDate)
        {
            if (!Context.User.IsInRole(nameof(Admin)) && (courierId.ToString() != Context.User.FindFirst("Id").Value)) return;

            var tracks = _dbContext.Set<Track>()
                                   .Where(t => (t.CourierId == courierId) && (t.TrackTime.Date == byDate.Date))
                                   .AsEnumerable();

            await Clients.Caller.SendAsync("GetTracksByDate", courierId, tracks);
        }
    }
}
