// MongoDB Playground
// Switch to the users database
use('sonixy_users');

// Update or Insert Client One
db.getCollection('users').updateOne(
    { _id: ObjectId("694139c16f1460bf247efe09") },
    {
        $set: {
            "FirstName": "Client",
            "LastName": "One",
            // "DisplayName": "Client One", // Computed property, do not store
            "Email": "client1@gmail.com",
            "Bio": "New user on Sonixy",
            "AvatarUrl": "",
            "CreatedAt": ISODate("2025-12-16T10:51:44.621Z"),
            "UpdatedAt": ISODate("2025-12-16T10:51:44.621Z")
        },
        $unset: { "DisplayName": "" }
    },
    { upsert: true }
);

// Update or Insert Client Two
db.getCollection('users').updateOne(
    { _id: ObjectId("69413f85d51d4a10fa4d6985") },
    {
        $set: {
            "FirstName": "Client",
            "LastName": "Two",
            // "DisplayName": "Client Two",
            "Email": "client2@gmail.com",
            "Bio": "Exploring the platform",
            "AvatarUrl": "",
            "CreatedAt": ISODate("2025-12-16T11:16:20.210Z"),
            "UpdatedAt": ISODate("2025-12-16T11:16:20.210Z")
        },
        $unset: { "DisplayName": "" }
    },
    { upsert: true }
);

// Update or Insert User B
db.getCollection('users').updateOne(
    { _id: ObjectId("694378ba7205e15245b90fbd") },
    {
        $set: {
            "FirstName": "User",
            "LastName": "B",
            // "DisplayName": "User B",
            "Email": "bbbb@gmail.com",
            "Bio": "Hello World",
            "AvatarUrl": "",
            "CreatedAt": ISODate("2025-12-18T03:44:58.193Z"),
            "UpdatedAt": ISODate("2025-12-18T03:44:58.193Z")
        },
        $unset: { "DisplayName": "" }
    },
    { upsert: true }
);

// Verify insertion
db.getCollection('users').find({
    _id: {
        $in: [
            ObjectId("694139c16f1460bf247efe09"),
            ObjectId("69413f85d51d4a10fa4d6985"),
            ObjectId("694378ba7205e15245b90fbd")
        ]
    }
});
