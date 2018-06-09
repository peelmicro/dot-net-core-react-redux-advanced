using System;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongodbIdentity.Models
{
    [CollectionName("blogs")]
    public class Blog: IDocument<ObjectId>
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
       
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("content")]
        public string Content { get; set; }
        [BsonElement("_user")]
        public ObjectId User { get; set; }
        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [BsonElement("__v")]
        public int Version { get; set; } = 0;

        public Blog(PostBlog postBlog, ObjectId user)
        {
            Title = postBlog.Title;
            Content = postBlog.Content;
            User = user;
        }
    }
}
