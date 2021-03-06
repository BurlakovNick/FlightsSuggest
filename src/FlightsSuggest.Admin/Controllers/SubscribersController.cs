﻿using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Admin.Infrastructure;
using FlightsSuggest.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightsSuggest.Admin.Controllers
{
    public class SubscribersController : Controller
    {
        private readonly IFlightsFunctions flightsFunctions;

        public SubscribersController(
            IFlightsFunctions flightsFunctions
            )
        {
            this.flightsFunctions = flightsFunctions;
        }

        public async Task<IActionResult> Index()
        {
            var subscribers = await flightsFunctions.SelectAsync().ConfigureAwait(true);
            var subscriberListViewModel = new SubscriberListViewModel
            {
                Subscribers = subscribers
                    .Select(x =>
                    {
                        var fullName = x.FirstName + " " + x.LastName;

                        return new SubscriberViewModel
                        {
                            Id = x.Id,
                            SendTelegramMessages = x.SendTelegramMessages,
                            NotificationTrigger = x.NotificationTrigger,
                            TelegramUsername = x.TelegramUsername,
                            TelegramChatId = x.TelegramChatId,
                            TelegramFullName = !string.IsNullOrEmpty(fullName) ? fullName : x.TelegramUsername,
                            IsBot = x.IsBot == true,
                        };
                    })
                    .ToArray()
            };

            return View("Index", subscriberListViewModel);
        }
    }
}