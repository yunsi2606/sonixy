/* global use, db */
// MongoDB Playground
// Update users with missing usernames

// Switch to the correct database
use('sonixy_users');

// Find all users without a username or with null username
const users = db.getCollection('users').find({
    $or: [
        { Username: { $exists: false } },
        { Username: null },
        { Username: "" }
    ]
}).toArray();

console.log(`Found ${users.length} users to update.`);

users.forEach(user => {
    let newUsername = "";

    if (user.Email) {
        // Try to use email prefix
        const emailPrefix = user.Email.split('@')[0];
        newUsername = emailPrefix;
    } else {
        // Fallback to ID if no email
        newUsername = `user_${user._id.toString()}`;
    }

    // Ensure uniqueness (basic check, appending random suffix if needed could be better but sticking to simple rule for now)
    // Check if this username already exists
    const exists = db.getCollection('users').findOne({ Username: newUsername, _id: { $ne: user._id } });
    if (exists) {
        newUsername = `${newUsername}_${Math.floor(Math.random() * 1000)}`;
    }

    db.getCollection('users').updateOne(
        { _id: user._id },
        {
            $set: {
                Username: newUsername,
                // Also update DisplayName if it's "Unknown User" or missing
                DisplayName: (!user.FirstName && !user.LastName) ? newUsername : (user.DisplayName === "Unknown User" ? newUsername : user.DisplayName)
            }
        }
    );

    console.log(`Updated user ${user._id}: Set Username to '${newUsername}'`);
});

console.log('Migration completed.');
