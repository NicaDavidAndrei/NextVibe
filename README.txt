How to run the application correctly:
Once you open the project in VS, run "Update-Database" in the Package Manager Console. This creates the database and applies all migrations. Seed data is inserted automatically when the application starts.

If artists, albums, tracks or roles do not appear correctly, reset the database:
Drop-Database
Update-Database

Administrator Account Setup:
The administrator account (admin@nextvibe.com) is created automatically on first run.
The password is NOT stored in the source code. You must configure it via .NET User Secrets before running the application.

To set the admin password locally using .NET User Secrets:

  dotnet user-secrets set "AdminSettings:Password" "YourChosenPassword" --project NextVibe.csproj

Replace "YourChosenPassword" with a password of at least 6 characters.

If the AdminSettings:Password secret is not set, the administrator account will not be created automatically on first run.

For non-development environments (e.g., production), set the following environment variable instead:
  AdminSettings__Password=YourChosenPassword

As a guest you can only access the homepage, explore, genres, moods and albums sections of the website.
An user has My Vibes, Recommendations, Profile sections added, while admin has everything.

Kanye West, Sabrina Carpenter, Metallica, Nirvana have discographies included on their details page.
Track lists are included for the albums of the following artists: Kanye West, Nirvana.

In order to test the recommendations you can add an artist that has a genre that is applied to two or more artists (for example: Sabrina Carpenter added to my vibes would result in The Weeknd being recommended).

The search function uses AJAX to return partial views for the filtering by genres/moods for an overall optimization of this feature.

The main implemented functionalities include:
- Authentication with login and register
- Authorization with user roles
- Role-based access restrictions
- Artist browsing
- Genre and mood browsing
- Artist details with discography
- Album details with track lists
- Favorites system through My Vibes
- Recommendation system based on favorite artist genres
- Admin panel with entity management
- CRUD operations for artists, genres, moods, albums and tracks
- Search and filtering functionality
