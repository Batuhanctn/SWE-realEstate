using MongoDB.Driver;
using Moq;
using RealEstate.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RealEstate.Tests.TestHelpers
{
    public class MockMongoDb
    {
        public Mock<IMongoCollection<Property>> PropertyCollection { get; }
        public Mock<IMongoCollection<User>> UserCollection { get; }
        public Mock<IMongoCollection<Favorite>> FavoriteCollection { get; }
        private List<Property> _properties;
        private List<User> _users;
        private List<Favorite> _favorites;

        public MockMongoDb()
        {
            _properties = new List<Property>();
            _users = new List<User>();
            _favorites = new List<Favorite>();

            PropertyCollection = new Mock<IMongoCollection<Property>>();
            UserCollection = new Mock<IMongoCollection<User>>();
            FavoriteCollection = new Mock<IMongoCollection<Favorite>>();

            SetupPropertyCollection();
            SetupUserCollection();
            SetupFavoriteCollection();
        }

        private void SetupPropertyCollection()
        {
            var mockAsyncCursor = new Mock<IAsyncCursor<Property>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(_properties);
            mockAsyncCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            PropertyCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<Property>>(),
                    It.IsAny<FindOptions<Property, Property>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);

            // Setup InsertOneAsync
            PropertyCollection
                .Setup(c => c.InsertOneAsync(
                    It.IsAny<Property>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Property, InsertOneOptions, CancellationToken>((property, options, token) =>
                {
                    if (string.IsNullOrEmpty(property.Id))
                    {
                        property.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                    }
                    _properties.Add(property);
                });

            // Setup ReplaceOneAsync
            PropertyCollection
                .Setup(c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<Property>>(),
                    It.IsAny<Property>(),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback<FilterDefinition<Property>, Property, ReplaceOptions, CancellationToken>(
                    (filter, replacement, options, token) =>
                    {
                        var index = _properties.FindIndex(p => p.Id == replacement.Id);
                        if (index != -1)
                        {
                            _properties[index] = replacement;
                        }
                    });

            // Setup DeleteOneAsync
            PropertyCollection
                .Setup(c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Property>>(),
                    It.IsAny<DeleteOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback<FilterDefinition<Property>, DeleteOptions, CancellationToken>(
                    (filter, options, token) =>
                    {
                        // Simple implementation - just remove the first matching item
                        var propertyToRemove = _properties.FirstOrDefault();
                        if (propertyToRemove != null)
                        {
                            _properties.Remove(propertyToRemove);
                        }
                    });
        }

        private void SetupUserCollection()
        {
            var mockAsyncCursor = new Mock<IAsyncCursor<User>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(_users);
            mockAsyncCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            UserCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);

            // Setup InsertOneAsync
            UserCollection
                .Setup(c => c.InsertOneAsync(
                    It.IsAny<User>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback<User, InsertOneOptions, CancellationToken>((user, options, token) =>
                {
                    if (string.IsNullOrEmpty(user.Id))
                    {
                        user.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                    }
                    _users.Add(user);
                });

            // Setup ReplaceOneAsync
            UserCollection
                .Setup(c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<User>(),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback<FilterDefinition<User>, User, ReplaceOptions, CancellationToken>(
                    (filter, replacement, options, token) =>
                    {
                        var index = _users.FindIndex(u => u.Id == replacement.Id);
                        if (index != -1)
                        {
                            _users[index] = replacement;
                        }
                    });
        }

        private void SetupFavoriteCollection()
        {
            var mockAsyncCursor = new Mock<IAsyncCursor<Favorite>>();
            mockAsyncCursor.Setup(_ => _.Current).Returns(_favorites);
            mockAsyncCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockAsyncCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            FavoriteCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<Favorite>>(),
                    It.IsAny<FindOptions<Favorite, Favorite>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);

            // Setup InsertOneAsync
            FavoriteCollection
                .Setup(c => c.InsertOneAsync(
                    It.IsAny<Favorite>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Favorite, InsertOneOptions, CancellationToken>((favorite, options, token) =>
                {
                    if (string.IsNullOrEmpty(favorite.Id))
                    {
                        favorite.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                    }
                    _favorites.Add(favorite);
                });

            // Setup ReplaceOneAsync
            FavoriteCollection
                .Setup(c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<Favorite>>(),
                    It.IsAny<Favorite>(),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback<FilterDefinition<Favorite>, Favorite, ReplaceOptions, CancellationToken>(
                    (filter, replacement, options, token) =>
                    {
                        var index = _favorites.FindIndex(f => f.Id == replacement.Id);
                        if (index != -1)
                        {
                            _favorites[index] = replacement;
                        }
                    });

            // Setup DeleteOneAsync
            FavoriteCollection
                .Setup(c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Favorite>>(),
                    It.IsAny<DeleteOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback<FilterDefinition<Favorite>, DeleteOptions, CancellationToken>(
                    (filter, options, token) =>
                    {
                        // Simple implementation - just remove the first matching item
                        var favoriteToRemove = _favorites.FirstOrDefault();
                        if (favoriteToRemove != null)
                        {
                            _favorites.Remove(favoriteToRemove);
                        }
                    });
        }

        public void AddProperty(Property property)
        {
            if (string.IsNullOrEmpty(property.Id))
            {
                property.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            }
            _properties.Add(property);
        }

        public void AddUser(User user)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            }
            _users.Add(user);
        }

        public void AddFavorite(Favorite favorite)
        {
            if (string.IsNullOrEmpty(favorite.Id))
            {
                favorite.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            }
            _favorites.Add(favorite);
        }

        public void ClearProperties()
        {
            _properties.Clear();
        }

        public void ClearUsers()
        {
            _users.Clear();
        }

        public void ClearFavorites()
        {
            _favorites.Clear();
        }
    }
}
