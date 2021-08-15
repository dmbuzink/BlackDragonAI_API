using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.Discord
{
    public class FakeDiscordManager : IDiscordManager
    {
        public Task Connect()
        {
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<StreamPlanning>> ReadStreamPlanning()
        {
            var sp1 = new StreamPlanning()
            {
                Date = new DateTime(2021,10, 26),
                TimeSlot = "Avond",
                Game = "Marvel’s Guardians of the Galaxy",
                StreamType = "Showcase of Let's Play",
                GameType = "Action Adventure / Story",
                TrailerUri = "https://youtu.be/QBn8ST8rELc"
            };
            var sp2 = new StreamPlanning()
            {
                Date = new DateTime(2021, 11, 5),
                TimeSlot = "Nacht, Middag & Avond",
                Game = "Forza Horizon 5",
                StreamType = "Racen met Daryll en Damian",
                GameType = "Racing / Open World",
                TrailerUri = "https://youtu.be/FYH9n37B7Yw"
            };
            var sp3 = new StreamPlanning()
            {
                Date = new DateTime(2021, 12, 7),
                TimeSlot = "Nacht & Avond",
                Game = "Dying Light 2",
                StreamType = "Showcase of Let's Play",
                GameType = "Zombie / Open World",
                TrailerUri = "https://youtu.be/UwJAAy7tPhE"
            };
            return new[]
            {
                sp1, sp2, sp3
            };
        }

        public Task WriteStreamPlanning(IEnumerable<StreamPlanning> streamPlannings)
        {
            return Task.CompletedTask;
        }
    }
}