using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BlackDragonAIAPI.Models;
using NUnit.Framework;

namespace Tests
{
    public class StreamPlanningParserTests
    {
        #region Parsing

        [Test]
        public void CanParseStreamPlanning_OneItem()
        {
            // Arrange
            string streamPlanningText = @"Do. 19-08  |  Avond  |  Twelve Minutes  |  Let's Play
Thriller / Story  |  Trailer: https://youtu.be/izoUXOMyVyk";
            
            // Act
            StreamPlanning parsedStreamPlanning = StreamPlanning.Parse(streamPlanningText);

            // Assert
            Assert.AreEqual(new DateTime(DateTime.Now.Year, 8, 19).Date, 
                parsedStreamPlanning.Date.Date);
            Assert.AreEqual("Avond", parsedStreamPlanning.TimeSlot);
            Assert.AreEqual("Twelve Minutes", parsedStreamPlanning.Game);
            Assert.AreEqual("Let's Play", parsedStreamPlanning.StreamType);
            Assert.AreEqual("Thriller / Story", parsedStreamPlanning.GameType);
            Assert.AreEqual("https://youtu.be/izoUXOMyVyk", parsedStreamPlanning.TrailerUri);
        }

        [Test]
        public void CanParseStreamPlanning_OneItem_NoTrailerAndNoGameType()
        {
            // Arrange
            string streamPlanningText = "Do. 19-08  |  Avond  |  Twelve Minutes  |  Let's Play";
            
            // Act
            StreamPlanning parsedStreamPlanning = StreamPlanning.Parse(streamPlanningText);

            // Assert
            Assert.AreEqual(new DateTime(DateTime.Now.Year, 8, 19).Date, 
                parsedStreamPlanning.Date.Date);
            Assert.AreEqual("Avond", parsedStreamPlanning.TimeSlot);
            Assert.AreEqual("Twelve Minutes", parsedStreamPlanning.Game);
            Assert.AreEqual("Let's Play", parsedStreamPlanning.StreamType);
            Assert.IsEmpty(parsedStreamPlanning.GameType);
            Assert.IsEmpty(parsedStreamPlanning.TrailerUri);
        }

        [Test]
        public void CanParseStreamPlanning_MultipleItems()
        {
            string streamPlanningText = @"Ma. 23-08  |  Avond  |  Riders Republic  |  Beta met Daryll en Damian
Open World / Sport  |  Trailer: https://youtu.be/ZPG1UIJjcks

Wo. 25-08  |  Avond  |  Psychonauts 2  |  Showcase of Let's Play
Action Adventure / Story  |  Trailer: https://youtu.be/ghO7GcYfrjc

Vr. 27-08  |  Avond  |  Baldo The Guardian Owls  |  Let's Play
Action Adventure / Open World  |  Trailer: https://youtu.be/7EgK5yLR37E

Di. 31-08  |  Avond  |  KeyWe  |  Let's Play met Damian
Coop / Puzzle  |  Trailer: https://youtu.be/54q_S-x8CCY";
            
            // Act
            ImmutableArray<StreamPlanning> parsedStreamPlannings = StreamPlanning.MultiParse(streamPlanningText)
                .ToImmutableArray();

            // Assert
            // SP1: Rider Republic
            StreamPlanning sp1 = parsedStreamPlannings.First(); 
            Assert.AreEqual(new DateTime(DateTime.Now.Year, 8, 23).Date, 
                sp1.Date.Date);
            Assert.AreEqual("Avond", sp1.TimeSlot);
            Assert.AreEqual("Riders Republic", sp1.Game);
            Assert.AreEqual("Beta met Daryll en Damian", sp1.StreamType);
            Assert.AreEqual("Open World / Sport", sp1.GameType);
            Assert.AreEqual("https://youtu.be/ZPG1UIJjcks", sp1.TrailerUri);

            // SP2: Psychonauts 2
            StreamPlanning sp2 = parsedStreamPlannings.Skip(1).First();
            Assert.AreEqual(new DateTime(DateTime.Now.Year, 8, 25).Date, 
                sp2.Date.Date);
            Assert.AreEqual("Avond", sp2.TimeSlot);
            Assert.AreEqual("Psychonauts 2", sp2.Game);
            Assert.AreEqual("Showcase of Let's Play", sp2.StreamType);
            Assert.AreEqual("Action Adventure / Story", sp2.GameType);
            Assert.AreEqual("https://youtu.be/ghO7GcYfrjc", sp2.TrailerUri);
            
            // SP3: Baldo The Guardian Owls 
            StreamPlanning sp3 = parsedStreamPlannings.Skip(2).First();
            Assert.AreEqual(new DateTime(DateTime.Now.Year, 8, 27).Date, 
                sp3.Date.Date);
            Assert.AreEqual("Avond", sp3.TimeSlot);
            Assert.AreEqual("Baldo The Guardian Owls", sp3.Game);
            Assert.AreEqual("Let's Play", sp3.StreamType);
            Assert.AreEqual("Action Adventure / Open World", sp3.GameType);
            Assert.AreEqual("https://youtu.be/7EgK5yLR37E", sp3.TrailerUri);
            
            // SP4: KeyWe 
            StreamPlanning sp4 = parsedStreamPlannings.Skip(3).First();
            Assert.AreEqual(new DateTime(DateTime.Now.Year, 8, 31).Date, 
                sp4.Date.Date);
            Assert.AreEqual("Avond", sp4.TimeSlot);
            Assert.AreEqual("KeyWe", sp4.Game);
            Assert.AreEqual("Let's Play met Damian", sp4.StreamType);
            Assert.AreEqual("Coop / Puzzle", sp4.GameType);
            Assert.AreEqual("https://youtu.be/54q_S-x8CCY", sp4.TrailerUri);
        }

        [Test]
        public void CanParseStreamPlanning_MultipleItems_ContainingUnParsable()
        {
            string streamPlanningText = @"Ma. 23-08  |  Avond  |  Riders Republic  |  Beta met Daryll en Damian
Open World / Sport  |  Trailer: https://youtu.be/ZPG1UIJjcks

Wo. 25-08  |  Avond  |  Psychonauts 2  |  Showcase of Let's Play
Action Adventure / Story  |  Trailer: https://youtu.be/ghO7GcYfrjc

//-12-21  |  Avond  |  OlliOlli World  |  Showcase
Skateboarden / Relaxen  |  Trailer: https://youtu.be/7RhBflGHxgw 

Vr. 27-08  |  Avond  |  Baldo The Guardian Owls  |  Let's Play
Action Adventure / Open World  |  Trailer: https://youtu.be/7EgK5yLR37E

Di. 31-08  |  Avond  |  KeyWe  |  Let's Play met Damian
Coop / Puzzle  |  Trailer: https://youtu.be/54q_S-x8CCY";
            
            // Act
            ImmutableArray<StreamPlanning> parsedStreamPlannings = StreamPlanning.MultiParse(streamPlanningText)
                .ToImmutableArray();

            // Assert
            Assert.AreEqual(4, parsedStreamPlannings.Length);
            // SP1: Rider Republic
            StreamPlanning sp1 = parsedStreamPlannings.First(); 
            Assert.AreEqual(new DateTime(DateTime.Now.Year, 8, 23).Date, 
                sp1.Date.Date);
            Assert.AreEqual("Avond", sp1.TimeSlot);
            Assert.AreEqual("Riders Republic", sp1.Game);
            Assert.AreEqual("Beta met Daryll en Damian", sp1.StreamType);
            Assert.AreEqual("Open World / Sport", sp1.GameType);
            Assert.AreEqual("https://youtu.be/ZPG1UIJjcks", sp1.TrailerUri);

            // SP2: Psychonauts 2
            StreamPlanning sp2 = parsedStreamPlannings.Skip(1).First();
            Assert.AreEqual(new DateTime(DateTime.Now.Year, 8, 25).Date, 
                sp2.Date.Date);
            Assert.AreEqual("Avond", sp2.TimeSlot);
            Assert.AreEqual("Psychonauts 2", sp2.Game);
            Assert.AreEqual("Showcase of Let's Play", sp2.StreamType);
            Assert.AreEqual("Action Adventure / Story", sp2.GameType);
            Assert.AreEqual("https://youtu.be/ghO7GcYfrjc", sp2.TrailerUri);
            
            // SP3: Baldo The Guardian Owls 
            StreamPlanning sp3 = parsedStreamPlannings.Skip(2).First();
            Assert.AreEqual(new DateTime(DateTime.Now.Year, 8, 27).Date, 
                sp3.Date.Date);
            Assert.AreEqual("Avond", sp3.TimeSlot);
            Assert.AreEqual("Baldo The Guardian Owls", sp3.Game);
            Assert.AreEqual("Let's Play", sp3.StreamType);
            Assert.AreEqual("Action Adventure / Open World", sp3.GameType);
            Assert.AreEqual("https://youtu.be/7EgK5yLR37E", sp3.TrailerUri);
            
            // SP4: KeyWe 
            StreamPlanning sp4 = parsedStreamPlannings.Skip(3).First();
            Assert.AreEqual(new DateTime(DateTime.Now.Year, 8, 31).Date, 
                sp4.Date.Date);
            Assert.AreEqual("Avond", sp4.TimeSlot);
            Assert.AreEqual("KeyWe", sp4.Game);
            Assert.AreEqual("Let's Play met Damian", sp4.StreamType);
            Assert.AreEqual("Coop / Puzzle", sp4.GameType);
            Assert.AreEqual("https://youtu.be/54q_S-x8CCY", sp4.TrailerUri);
        }

        #endregion

        #region Serializing

        [Test]
        public void SerializeStreamPlanning_OneItem()
        {
            // Arrange
            var streamPlanning = new StreamPlanning()
            {
                Date = new DateTime(2021, 11, 5),
                TimeSlot = "Nacht, Middag & Avond",
                Game = "Forza Horizon 5",
                StreamType = "Racen met Daryll en Damian",
                GameType = "Racing / Open World",
                TrailerUri = "https://youtu.be/FYH9n37B7Yw"
            };

            // Act
            string serializedStreamPlanning = streamPlanning.ToString();

            // Assert
            Assert.AreEqual(@"Vr. 05-11  |  Nacht, Middag & Avond  |  Forza Horizon 5  |  Racen met Daryll en Damian
Racing / Open World  |  Trailer: https://youtu.be/FYH9n37B7Yw", 
                serializedStreamPlanning);
        }

        [Test]
        public void SerializeStreamPlanning_MultipleItems()
        {
            // Arrange
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
            
            // Act
            string serializedStreamPlannings = StreamPlanning.SerializeMultiple(new []{sp1, sp2, sp3});
            
            // Assert
            Assert.AreEqual(@"Di. 26-10  |  Avond  |  Marvel’s Guardians of the Galaxy  |  Showcase of Let's Play
Action Adventure / Story  |  Trailer: https://youtu.be/QBn8ST8rELc

Vr. 05-11  |  Nacht, Middag & Avond  |  Forza Horizon 5  |  Racen met Daryll en Damian
Racing / Open World  |  Trailer: https://youtu.be/FYH9n37B7Yw

Di. 07-12  |  Nacht & Avond  |  Dying Light 2  |  Showcase of Let's Play
Zombie / Open World  |  Trailer: https://youtu.be/UwJAAy7tPhE", 
                serializedStreamPlannings);
        }

        #endregion
    }
}