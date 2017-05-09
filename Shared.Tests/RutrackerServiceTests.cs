using System;
using System.Linq;
using NUnit.Framework;
using PersonalBot.Shared.Domain.Torrents.Services;

namespace PersonalBot.Shared.Tests
{
    [TestFixture]
    public class RutrackerServiceTests
    {
        [Test]
        public void ShouldGetTorrentInfoById()
        {
            // arrange
            var ids = new[] { "2142" };

            // act
            var sut = new RutrackerService();
            var topics = sut.GetTopicsAsync(ids).Result;

            // assert
            Assert.AreEqual(1, topics.Count);
            var topic = topics.Single();
            Assert.AreEqual("2142", topic.Id);
            Assert.AreEqual("658EDAB6AF0B424E62FEFEC0E39DBE2AC55B9AE3", topic.InfoHash);
            Assert.AreEqual("Гражданин начальник / Сезон: 1 / Серии: 1-15 из 15 (Николай Досталь) [2001, драма, криминал, TVRip]", topic.TopicTitle);
            Assert.AreEqual(new DateTime(2005, 04, 08, 02, 51, 36), topic.RegTime);
        }

        [Test]
        public void ShouldSkipAbsentTopics()
        {
            // arrange
            var ids = new[] { "1","2142" };

            // act
            var sut = new RutrackerService();
            var topics = sut.GetTopicsAsync(ids).Result;

            // assert
            Assert.AreEqual(1, topics.Count);
            var topic = topics.Single();
            Assert.AreEqual("2142", topic.Id);
        }

        [Test]
        public void ShouldGetAllNeededIds()
        {
            // arrange
            var ids = "5285924,5387197,5397016".Split(',');

            // act 
            var sut = new RutrackerService();
            var topics = sut.GetTopicsAsync(ids).Result;

            // assert
            Assert.That(topics.Count, Is.EqualTo(ids.Length));
        }
    }
}
