using System;
using NUnit.Framework;
using PersonalBot.Shared.Domain.Torrents.Models;

namespace PersonalBot.Shared.Tests
{
    [TestFixture]
    public class TorrentPresenterTests
    {
        [TestCase(
            "Лучше звоните Солу / Better Call Saul / Сезон: 3 / Серии: 1-3 из 10 (Колин Бакси, Адам Бернштейн) [2017, США, драма, комедия, криминал, WEB-DL 1080p] MVO (NewStudio) + Original + Subs (Rus,Eng)",
            "Лучше звоните Солу")]
        [TestCase(
            "Плохо оформленный топик",
            "Плохо оформленный топик")]
        public void ShouldExtractTitle(string title, string expectedResult)
        {
            // arrange
            var topic = CreateTopic(topicTitle: title);

            // act 
            var sut = new TorrentPresenter(topic);
            Assert.That(sut.Title, Is.EqualTo(expectedResult));
        }

        [TestCase(
            "Лучше звоните Солу / Better Call Saul / Сезон: 3 / Серии: 1-3 из 10 (Колин Бакси, Адам Бернштейн) [2017, США, драма, комедия, криминал, WEB-DL 1080p] MVO (NewStudio) + Original + Subs (Rus,Eng)",
            "Серии: 1-3 из 10")]
        [TestCase(
            "Чёрные паруса / Black Sails / Сезон 4 / Серии 1-10 (10) (Нил Маршалл) [2017, США, ЮАР, драма, приключения, HDTV 720p] MVO (AlexFilm)",
            "Серии 1-10 (10)")]
        [TestCase(
            "Плохо оформленный топик",
            "Серии: нет данных")]
        public void ShouldExtractSeries(string title, string expectedResult)
        {
            // arrange
            var topic = CreateTopic(topicTitle: title);

            // act 
            var sut = new TorrentPresenter(topic);
            Assert.That(sut.Series, Is.EqualTo(expectedResult));
        }

        [TestCase(1, 1, "1 day, 1 hour ago")]
        [TestCase(2, 0, "2 days ago")]
        [TestCase(0, 5, "5 hours ago")]
        [TestCase(0, 1, "now")]
        public void ShoudSetUpdated(int days, int hours, string expectedResult)
        {
            // arrange
            var regTime = DateTime.Now.AddDays(-days).AddHours(-hours);
            var topic = CreateTopic(regTime: regTime);

            // act 
            var sut = new TorrentPresenter(topic);
            Assert.That(sut.Updated, Is.EqualTo(expectedResult));
        }

        [TestCase]
        public void ShoudNotSetIsSeenWhenTorrentNotProvided()
        {
            // arrange
            var topic = CreateTopic();
            var torrent = default(TorrentEntity);

            // act 
            var sut = new TorrentPresenter(topic, torrent);
            Assert.That(sut.IsSeen, Is.EqualTo(false));
        }
        
        [TestCase]
        public void ShoudSetIsSeen()
        {
            // arrange
            var topic = CreateTopic();
            var torrent = CreateTorrent();
            torrent.IsSeen = true;

            // act 
            var sut = new TorrentPresenter(topic, torrent);
            Assert.That(sut.IsSeen, Is.EqualTo(true));
        }

        private TorrentEntity CreateTorrent()
        {
            return new TorrentEntity();
        }

        private static Topic CreateTopic(
            string id = null,
            string topicTitle = null, 
            DateTime? regTime = null)
        {
            return new Topic
            {
                Id = id ?? "123",
                TopicTitle = topicTitle ?? "test",
                RegTime = regTime ?? DateTime.Now
            };
        }
    }
}
