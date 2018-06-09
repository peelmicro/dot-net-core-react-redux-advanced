using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;

public class ApplicationRole : MongoIdentityRole<ObjectId>
{
	public ApplicationRole() : base()
	{
	}

	public ApplicationRole(string roleName) : base(roleName)
	{
	}
}	