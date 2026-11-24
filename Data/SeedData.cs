using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NextVibe.Models;

namespace NextVibe.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();

            var services = scope.ServiceProvider;

            var db = services.GetRequiredService<AppDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            await db.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager, configuration);
            await SeedGenresAsync(db);
            await SeedMoodsAsync(db);
            await SeedArtistsAsync(db);
            await SeedKenCarsonAsync(db);
            await SeedDonToliverAsync(db);
            await SeedArtistAlbumsAsync(db);
            await SeedAlbumTracksAsync(db);
            await SeedArtistMoodsAsync(db);
            await SeedLyricChallengesAsync(db);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            var adminEmail = "admin@nextvibe.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var adminPassword = configuration["AdminSettings:Password"];

                if (string.IsNullOrWhiteSpace(adminPassword))
                {
                    // No password configured – skip admin creation safely.
                    return;
                }

                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "NextVibe",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        private static async Task SeedGenresAsync(AppDbContext db)
        {
            if (!await db.Genres.AnyAsync())
            {
                db.Genres.AddRange(
                    new Genre { Name = "Hip-Hop", Description = "Hip-hop is a music genre built around rhythmic vocals, beats, sampling, and urban storytelling. It often focuses on personal experience, social issues, confidence, and lyrical expression." },
                    new Genre { Name = "Pop", Description = "Pop is a broad mainstream genre known for catchy melodies, accessible song structures, polished production, and strong hooks designed to appeal to a wide audience." },
                    new Genre { Name = "Rock", Description = "Rock is a genre centered around electric guitars, drums, bass, and strong rhythms. It includes many substyles, from classic rock to alternative and heavier guitar-driven sounds." },
                    new Genre { Name = "Electronic", Description = "Electronic music is produced mainly with synthesizers, drum machines, digital instruments, and computer-based production. It often emphasizes rhythm, texture, and atmosphere." },
                    new Genre { Name = "R&B", Description = "Rhythm and Blues is a genre focused on soulful vocals, smooth melodies, groove-based instrumentals, and emotional themes such as love, relationships, and personal reflection." },
                    new Genre { Name = "Indie", Description = "Indie music refers to music created with an independent or alternative spirit. It often values creative freedom, personal songwriting, and less conventional production choices." },
                    new Genre { Name = "Alternative", Description = "Alternative music includes styles that developed outside mainstream conventions. It often mixes rock, indie, electronic, experimental, and emotionally expressive elements." },
                    new Genre { Name = "Trap", Description = "Trap is a subgenre of hip-hop known for heavy bass, sharp hi-hats, dark melodies, and rhythmic vocal flows. It often has an intense, modern, and atmospheric sound." },
                    new Genre { Name = "Psychedelic", Description = "Psychedelic music is characterized by dreamy textures, immersive soundscapes, unusual effects, and experimental production meant to create a surreal or mind-expanding atmosphere." },
                    new Genre { Name = "Soul", Description = "Soul is a genre that emphasizes expressive vocals, emotional delivery, groove, and rich musical arrangements. It often blends gospel, R&B, jazz, and blues influences." },
                    new Genre { Name = "Grunge", Description = "Grunge is a subgenre of alternative rock and a youth subculture that emerged in the mid-1980s in Washington state, particularly Seattle, peaking in popularity during the early 1990s. Often termed the Seattle Sound, it is characterized by a dirty or sludgy sound that fuses elements of punk rock and heavy metal, utilizing distorted guitars, heavy basslines, and often angst-filled, introspective lyrics." },
                    new Genre { Name = "Heavy Metal", Description = "Heavy metal is a loud and powerful rock genre built around distorted guitars, strong riffs, intense drums, and dramatic vocals.It often emphasizes energy, heaviness, and technical musicianship." }
                );

                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedMoodsAsync(AppDbContext db)
        {
            if (!await db.Moods.AnyAsync())
            {
                db.Moods.AddRange(
                    new Mood { Name = "Chill", Description = "Relaxed and calm music, usually soft, smooth, and easy to listen to." },
                    new Mood { Name = "Energetic", Description = "High energy and dynamic music, usually fast-paced and motivating." },
                    new Mood { Name = "Sad", Description = "Emotional and reflective music, often focused on loss, regret, loneliness, or vulnerability." },
                    new Mood { Name = "Dark", Description = "Moody, intense, and atmospheric music with a heavier or more mysterious feeling." },
                    new Mood { Name = "Dreamy", Description = "Floating, soft, and atmospheric music that creates an immersive or surreal feeling." },
                    new Mood { Name = "Motivational", Description = "Empowering and uplifting music that creates confidence, ambition, and drive." },
                    new Mood { Name = "Melancholic", Description = "Soft sadness with emotional depth, nostalgia, and introspective atmosphere." },
                    new Mood { Name = "Aggressive", Description = "Hard-hitting and intense music with powerful delivery, strong rhythms, or heavy production." },
                    new Mood { Name = "Romantic", Description = "Warm, intimate, and emotional music focused on love, affection, and relationships." },
                    new Mood { Name = "Experimental", Description = "Unconventional music that explores unusual sounds, structures, production choices, or artistic ideas." }
                );

                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedArtistsAsync(AppDbContext db)
        {
            if (!await db.Artists.AnyAsync())
            {
                var hiphop = await db.Genres.FirstAsync(g => g.Name == "Hip-Hop");
                var pop = await db.Genres.FirstAsync(g => g.Name == "Pop");
                var rock = await db.Genres.FirstAsync(g => g.Name == "Rock");
                var electronic = await db.Genres.FirstAsync(g => g.Name == "Electronic");
                var rnb = await db.Genres.FirstAsync(g => g.Name == "R&B");
                var indie = await db.Genres.FirstAsync(g => g.Name == "Indie");
                var alternative = await db.Genres.FirstAsync(g => g.Name == "Alternative");
                var trap = await db.Genres.FirstAsync(g => g.Name == "Trap");
                var psychedelic = await db.Genres.FirstAsync(g => g.Name == "Psychedelic");
                var soul = await db.Genres.FirstAsync(g => g.Name == "Soul");
                var grunge = await db.Genres.FirstAsync(g => g.Name == "Grunge");
                var heavyMetal = await db.Genres.FirstAsync(g => g.Name == "Heavy Metal");

                db.Artists.AddRange(
                    new Artist { Name = "Kendrick Lamar", Bio = "Influential hip-hop artist known for complex lyricism, storytelling, and socially conscious themes.", ImageUrl = "/images/kendrick-lamar.jpg", Country = "USA", BirthYear = 1987, GenreId = hiphop.Id },
                    new Artist { Name = "Kanye West", Bio = "One of the most influential hip-hop artists and producers, known for constantly changing his sound and visual identity.", ImageUrl = "/images/kanye-west.jpg", Country = "USA", BirthYear = 1977, GenreId = hiphop.Id },
                    new Artist { Name = "The Weeknd", Bio = "Modern pop and R&B artist known for atmospheric production, emotional vocals, and cinematic albums.", ImageUrl = "/images/weeknd.jpg", Country = "Canada", BirthYear = 1990, GenreId = pop.Id },
                    new Artist { Name = "Tame Impala", Bio = "Psychedelic indie project known for dreamy production, layered instruments, and immersive soundscapes.", ImageUrl = "/images/tame-impala.jpg", Country = "Australia", BirthYear = 1986, GenreId = psychedelic.Id },
                    new Artist { Name = "Sabrina Carpenter", Bio = "Pop artist known for catchy melodies, polished production, and confident modern pop songwriting.", ImageUrl = "/images/sabrina-carpenter.jpg", Country = "USA", BirthYear = 1999, GenreId = pop.Id },
                    new Artist { Name = "Nirvana", Bio = "Legendary grunge band that helped define the Seattle sound and alternative rock in the early 1990s.", ImageUrl = "/images/nirvana.jpg", Country = "USA", BirthYear = 1987, GenreId = grunge.Id },
                    new Artist { Name = "Travis Scott", Bio = "Trap artist known for atmospheric production, energetic performances, and psychedelic hip-hop influences.", ImageUrl = "/images/travis-scott.jpg", Country = "USA", BirthYear = 1991, GenreId = trap.Id },
                    new Artist { Name = "Frank Ocean", Bio = "Critically acclaimed R&B artist known for emotional songwriting, experimental production, and introspective lyrics.", ImageUrl = "/images/frank-ocean.jpg", Country = "USA", BirthYear = 1987, GenreId = rnb.Id },
                    new Artist { Name = "Amy Winehouse", Bio = "Soul and jazz artist known for her distinctive voice, emotional lyrics, and retro-inspired sound.", ImageUrl = "/images/amy-winehouse.jpg", Country = "United Kingdom", BirthYear = 1983, GenreId = soul.Id },
                    new Artist { Name = "Daft Punk", Bio = "Legendary electronic duo known for house, disco, funk influences, robotic visuals, and innovative production.", ImageUrl = "/images/daft-punk.jpg", Country = "France", BirthYear = 1993, GenreId = electronic.Id },
                    new Artist { Name = "Sleep Token", Bio = "Anonymous music collective blending alternative metal, electronic textures, pop melodies, and atmospheric emotion.", ImageUrl = "/images/sleep-token.jpg", Country = "United Kingdom", BirthYear = 2016, GenreId = alternative.Id },
                    new Artist { Name = "Arctic Monkeys", Bio = "Indie rock band known for sharp lyrics, stylish guitar-driven music, and evolving alternative rock sound.", ImageUrl = "/images/arctic-monkeys.jpg", Country = "United Kingdom", BirthYear = 2002, GenreId = indie.Id },
                    new Artist { Name = "Metallica", Bio = "Legendary heavy metal band known for powerful riffs, aggressive energy, and some of the most influential albums in metal history", ImageUrl = "/images/artists/metallica.jpg", Country = "USA", BirthYear = 1981, GenreId = heavyMetal.Id },
                    new Artist { Name = "Queen", Bio = "Iconic rock band known for their theatrical style, powerful vocals, and genre-blending music that has become timeless classics.", ImageUrl = "/images/artists/queen.jpg", Country = "United Kingdom", BirthYear = 1970, GenreId = rock.Id },
                    new Artist { Name = "Tyler, The Creator", Bio = "Innovative hip-hop artist and producer known for his unique style, creative vision, and genre-blending music.", ImageUrl = "/images/artists/tyler-the-creator.jpg", Country = "USA", BirthYear = 1991, GenreId = rnb.Id }
                );

                await db.SaveChangesAsync();
            }

            await UpdateArtistImagesAsync(db);
        }

        private static async Task SeedKenCarsonAsync(AppDbContext db)
        {
            var ken = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Ken Carson");
            if (ken == null)
            {
                var trap = await db.Genres.FirstOrDefaultAsync(g => g.Name == "Trap");
                if (trap == null)
                {
                    return;
                }

                db.Artists.Add(new Artist
                {
                    Name = "Ken Carson",
                    Bio = "American rapper known for his energetic rage rap sound, distorted electronic production, aggressive vocal delivery, and association with the Opium collective.",
                    Country = "USA",
                    BirthYear = 2000,
                    GenreId = trap.Id,
                    ImageUrl = "/images/artists/ken-carson.jpg"
                });

                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedDonToliverAsync(AppDbContext db)
        {
            var don = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Don Toliver");
            if (don == null)
            {
                var trap = await db.Genres.FirstOrDefaultAsync(g => g.Name == "Trap");
                if (trap == null) return;

                db.Artists.Add(new Artist
                {
                    Name = "Don Toliver",
                    Bio = "American singer and rapper known for his unique melodic vocal styling, blending trap, R&B, and hip-hop. Prominent member of Cactus Jack Records.",
                    Country = "USA",
                    BirthYear = 1994,
                    GenreId = trap.Id,
                    ImageUrl = "/images/artists/don-toliver.jpg"
                });

                await db.SaveChangesAsync();
            }
        }

        private static async Task UpdateArtistImagesAsync(AppDbContext db)
        {
            var imageMap = new Dictionary<string, string>
            {
                { "Kendrick Lamar", "/images/artists/kendrick-lamar.jpg" },
                { "Kanye West", "/images/artists/kanye-west.jpg" },
                { "The Weeknd", "/images/artists/weeknd.jpg" },
                { "Tame Impala", "/images/artists/tame-impala.jpg" },
                { "Sabrina Carpenter", "/images/artists/sabrina-carpenter.jpg" },
                { "Nirvana", "/images/artists/nirvana.jpg" },
                { "Travis Scott", "/images/artists/travis-scott.jpg" },
                { "Frank Ocean", "/images/artists/frank-ocean.jpg" },
                { "Amy Winehouse", "/images/artists/amy-winehouse.jpg" },
                { "Daft Punk", "/images/artists/daft-punk.jpg" },
                { "Sleep Token", "/images/artists/sleep-token.jpg" },
                { "Arctic Monkeys", "/images/artists/arctic-monkeys.jpg" },
                { "Metallica", "/images/artists/metallica.jpg" },
                { "Queen", "/images/artists/queen.jpg" },
                { "Tyler, The Creator", "/images/artists/tyler-the-creator.jpg" },
                { "Ken Carson", "/images/artists/ken-carson.jpg" },
                { "Don Toliver", "/images/artists/don-toliver.jpg" }
            };

            foreach (var item in imageMap)
            {
                var artist = await db.Artists.FirstOrDefaultAsync(a => a.Name == item.Key);

                if (artist != null)
                {
                    artist.ImageUrl = item.Value;
                }
            }

            await db.SaveChangesAsync();
        }

        private static async Task SeedArtistAlbumsAsync(AppDbContext db)
        {
            var kanye = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Kanye West");
            var sabrina = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Sabrina Carpenter");
            var metallica = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Metallica");
            var nirvana = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Nirvana");
            var kendrick = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Kendrick Lamar");
            var ken = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Ken Carson");
            var don = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Don Toliver");

            if (kanye == null)
            {
                return;
            }

            if (sabrina == null)
            {
                return;
            }

            if (metallica == null)
            {
                return;
            }

            if (nirvana == null)
            {
                return;
            }

            var albumsToSeed = new List<ArtistAlbum>
            {
                // Kanye West Albums
                new ArtistAlbum { Title = "The College Dropout", ArtistId = kanye.Id, ReleaseYear = 2004, Description = "Kanye West's debut album that revolutionized hip-hop with soulful samples and introspective lyrics about education, faith, and ambition.", CoverImageUrl = "/images/albums/the-college-dropout.jpg" },
                new ArtistAlbum { Title = "Late Registration", ArtistId = kanye.Id, ReleaseYear = 2005, Description = "A more orchestral and refined follow-up featuring lush production and deeper storytelling.", CoverImageUrl = "/images/albums/late-registration.jpg" },
                new ArtistAlbum { Title = "Graduation", ArtistId = kanye.Id, ReleaseYear = 2007, Description = "A stadium-sized hip-hop album blending electronic sounds with mainstream appeal.", CoverImageUrl = "/images/albums/graduation.jpg" },
                new ArtistAlbum { Title = "808s & Heartbreak", ArtistId = kanye.Id, ReleaseYear = 2008, Description = "A minimal, emotional album that introduced heavy use of Auto-Tune and influenced a generation of artists.", CoverImageUrl = "/images/albums/808s-and-heartbreak.jpg" },
                new ArtistAlbum { Title = "My Beautiful Dark Twisted Fantasy", ArtistId = kanye.Id, ReleaseYear = 2010, Description = "Widely considered Kanye's masterpiece, combining grand production with deeply personal and controversial themes.", CoverImageUrl = "/images/albums/my-beautiful-dark-twisted-fantasy.jpg" },
                new ArtistAlbum { Title = "Yeezus", ArtistId = kanye.Id, ReleaseYear = 2013, Description = "An aggressive and experimental album with industrial influences and minimalist production.", CoverImageUrl = "/images/albums/yeezus.jpg" },
                new ArtistAlbum { Title = "The Life of Pablo", ArtistId = kanye.Id, ReleaseYear = 2016, Description = "A chaotic and evolving album blending gospel, hip-hop, and electronic elements.", CoverImageUrl = "/images/albums/the-life-of-pablo.jpg" },
                new ArtistAlbum { Title = "Ye", ArtistId = kanye.Id, ReleaseYear = 2018, Description = "A short and introspective album focused on mental health and personal struggles.", CoverImageUrl = "/images/albums/ye.jpg" },
                new ArtistAlbum { Title = "Jesus Is King", ArtistId = kanye.Id, ReleaseYear = 2019, Description = "A gospel-inspired album reflecting Kanye's spiritual transformation.", CoverImageUrl = "/images/albums/jesus-is-king.jpg" },
                new ArtistAlbum { Title = "Donda", ArtistId = kanye.Id, ReleaseYear = 2021, Description = "A massive and ambitious album dedicated to his mother, blending multiple styles and themes.", CoverImageUrl = "/images/albums/donda.jpg" },
                new ArtistAlbum { Title = "Donda 2", ArtistId = kanye.Id, ReleaseYear = 2022, Description = "A sequel to Donda released through unconventional platforms, continuing themes of loss, identity, and experimentation.", CoverImageUrl = "/images/albums/donda-2.jpg" },
                new ArtistAlbum { Title = "Bully", ArtistId = kanye.Id, ReleaseYear = 2026, Description = "A bold and experimental album from Kanye West, continuing his evolution into darker and more unconventional sounds.", CoverImageUrl = "/images/albums/bully.jpg" },

                // Sabrina Carpenter Albums
                new ArtistAlbum { Title = "Eyes Wide Open", ArtistId = sabrina.Id, ReleaseYear = 2015, Description = "Sabrina Carpenter's debut album featuring a blend of pop and R&B influences.", CoverImageUrl = "/images/albums/eyes-wide-open.jpg" },
                new ArtistAlbum { Title = "EVOLution", ArtistId = sabrina.Id, ReleaseYear = 2016, Description = "A more mature and experimental album with darker pop sounds.", CoverImageUrl = "/images/albums/evolution.jpg" },
                new ArtistAlbum { Title = "Singular: Act I", ArtistId = sabrina.Id, ReleaseYear = 2018, Description = "A concept album exploring themes of identity and self-discovery with a mix of pop and R&B.", CoverImageUrl = "/images/albums/singular-act-1.jpg" },
                new ArtistAlbum { Title = "Singular: Act II", ArtistId = sabrina.Id, ReleaseYear = 2019, Description = "The continuation of the Singular concept, featuring more experimental production and introspective lyrics.", CoverImageUrl = "/images/albums/singular-act-2.jpg" },
                new ArtistAlbum { Title = "Emails I Can't Send", ArtistId = sabrina.Id, ReleaseYear = 2022, Description = "A deeply personal album with a mix of pop, R&B, and indie influences.", CoverImageUrl = "/images/albums/emails-i-cant-send.jpg" },
                new ArtistAlbum { Title = "Short n' Sweet", ArtistId = sabrina.Id, ReleaseYear = 2024, Description = "A collection of short and experimental tracks showcasing Sabrina's evolving sound.", CoverImageUrl = "/images/albums/short-n-sweet.jpg" },
                new ArtistAlbum { Title = "Man's Best Friend", ArtistId = sabrina.Id, ReleaseYear = 2025, Description = "A concept album exploring themes of companionship, loyalty, and self-reflection with a mix of pop and alternative sounds.", CoverImageUrl = "/images/albums/mans-best-friend.jpg" },

                // Metallica Albums
                new ArtistAlbum { Title = "Kill 'Em All", ArtistId = metallica.Id, ReleaseYear = 1983, Description = "Metallica's debut album that helped define the thrash metal genre with its aggressive riffs and raw energy.", CoverImageUrl = "/images/albums/kill-em-all.jpg" },
                new ArtistAlbum { Title = "Ride the Lightning", ArtistId = metallica.Id, ReleaseYear = 1984, Description = "A more complex and refined album that solidified Metallica's place in metal history.", CoverImageUrl = "/images/albums/ride-the-lightning.jpg" },
                new ArtistAlbum { Title = "Master of Puppets", ArtistId = metallica.Id, ReleaseYear = 1986, Description = "Widely regarded as one of the greatest metal albums of all time, featuring intricate compositions and powerful themes.", CoverImageUrl = "/images/albums/master-of-puppets.jpg" },
                new ArtistAlbum { Title = "...And Justice for All", ArtistId = metallica.Id, ReleaseYear = 1988, Description = "A politically charged album with complex arrangements and a more progressive sound.", CoverImageUrl = "/images/albums/and-justice-for-all.jpg" },
                new ArtistAlbum { Title = "Metallica (The Black Album)", ArtistId = metallica.Id, ReleaseYear = 1991, Description = "A more mainstream and accessible album that became Metallica's best-selling record.", CoverImageUrl = "/images/albums/metallica.jpg" },
                new ArtistAlbum { Title = "Load", ArtistId = metallica.Id, ReleaseYear = 1996, Description = "A departure from their thrash roots, featuring a more bluesy and hard rock sound.", CoverImageUrl = "/images/albums/load.jpg" },
                new ArtistAlbum { Title = "Reload", ArtistId = metallica.Id, ReleaseYear = 1997, Description = "A continuation of the Load era with more experimentation and a mix of hard rock and metal.", CoverImageUrl = "/images/albums/reload.jpg" },
                new ArtistAlbum { Title = "St. Anger", ArtistId = metallica.Id, ReleaseYear = 2003, Description = "A raw and aggressive album that received mixed reviews but showcased the band's willingness to experiment.", CoverImageUrl = "/images/albums/st-anger.jpg" },
                new ArtistAlbum { Title = "Death Magnetic", ArtistId = metallica.Id, ReleaseYear = 2008, Description = "A return to their thrash metal roots with a modern production style.", CoverImageUrl = "/images/albums/death-magnetic.jpg" },
                new ArtistAlbum { Title = "Hardwired... to Self-Destruct", ArtistId = metallica.Id, ReleaseYear = 2016, Description = "A double album that continues Metallica's legacy with a mix of thrash and hard rock elements.", CoverImageUrl = "/images/albums/hardwired.jpg" },
                new ArtistAlbum { Title = "72 Seasons", ArtistId = metallica.Id, ReleaseYear = 2023, Description = "An album exploring themes of personal growth and reflection, blending Metallica's signature sound with new influences.", CoverImageUrl = "/images/albums/72-seasons.jpg" },

                // Nirvana Albums
                new ArtistAlbum { Title = "Bleach", ArtistId = nirvana.Id, ReleaseYear = 1989, Description = "Nirvana's debut album that introduced their raw grunge sound and dark themes.", CoverImageUrl = "/images/albums/bleach.jpg" },
                new ArtistAlbum { Title = "Nevermind", ArtistId = nirvana.Id, ReleaseYear = 1991, Description = "The album that catapulted Nirvana to fame, featuring iconic tracks and a more polished sound.", CoverImageUrl = "/images/albums/nevermind.jpg" },
                new ArtistAlbum { Title = "In Utero", ArtistId = nirvana.Id, ReleaseYear = 1993, Description = "A more abrasive and experimental album that showcased Nirvana's artistic growth.", CoverImageUrl = "/images/albums/in-utero.jpg" }
            };

            if (kendrick != null)
            {
                albumsToSeed.AddRange(new[]
                {
                    // Kendrick Lamar Albums
                    new ArtistAlbum { Title = "Overly Dedicated", ArtistId = kendrick.Id, ReleaseYear = 2010, Description = "Kendrick Lamar's breakthrough mixtape that established his reflective lyricism, intricate rhyming, and soulful West Coast storytelling.", CoverImageUrl = "/images/albums/Kendrick_Lamar/Overly_Dedicated.jpg" },
                    new ArtistAlbum { Title = "Section.80", ArtistId = kendrick.Id, ReleaseYear = 2011, Description = "A concept debut studio album tackling the generational vices, struggles, and mindset of youths born in the 1980s.", CoverImageUrl = "/images/albums/Kendrick_Lamar/Section_80.jpg" },
                    new ArtistAlbum { Title = "good kid, m.A.A.d city (deluxe)", ArtistId = kendrick.Id, ReleaseYear = 2012, Description = "A cinematic hip-hop narrative detailing Kendrick's teenage years navigating family, peer pressure, and violence in Compton.", CoverImageUrl = "/images/albums/Kendrick_Lamar/good_kid_mAAd_city.jpg" },
                    new ArtistAlbum { Title = "To Pimp a Butterfly", ArtistId = kendrick.Id, ReleaseYear = 2015, Description = "An ambitious and critically acclaimed masterpiece blending jazz, funk, and spoken word to confront racial injustice, mental health, and institutional oppression.", CoverImageUrl = "/images/albums/Kendrick_Lamar/To_Pimp_A_Butterfly.jpg" },
                    new ArtistAlbum { Title = "untitled unmastered.", ArtistId = kendrick.Id, ReleaseYear = 2016, Description = "A collection of raw, unfinished, and deeply funky studio session demos recorded during the To Pimp a Butterfly era.", CoverImageUrl = "/images/albums/Kendrick_Lamar/untitled_unmastered.jpg" },
                    new ArtistAlbum { Title = "DAMN.", ArtistId = kendrick.Id, ReleaseYear = 2017, Description = "A Pulitzer Prize-winning album contrasting wicked human impulses against spiritual righteousness with sharp, modern trap production.", CoverImageUrl = "/images/albums/Kendrick_Lamar/DAMN.jpg" },
                    new ArtistAlbum { Title = "Black Panther The Album Music From And Inspired By", ArtistId = kendrick.Id, ReleaseYear = 2018, Description = "A soundtrack curated and produced by Kendrick Lamar for Marvel's Black Panther, fusing African pop, trap, and contemporary R&B.", CoverImageUrl = "/images/albums/Kendrick_Lamar/Black_Panther.jpg" },
                    new ArtistAlbum { Title = "Mr. Morale & The Big Steppers", ArtistId = kendrick.Id, ReleaseYear = 2022, Description = "A raw, theatrical double album exploring therapy, generational trauma, accountability, and spiritual growth.", CoverImageUrl = "/images/albums/Kendrick_Lamar/Mr_Morale_&_The_Big_Steppers.jpg" },
                    new ArtistAlbum { Title = "GNX", ArtistId = kendrick.Id, ReleaseYear = 2024, Description = "A celebratory tribute to West Coast hip-hop culture, Los Angeles heritage, and independence named after the iconic 1987 Buick Grand National GNX.", CoverImageUrl = "/images/albums/Kendrick_Lamar/GNX.jpg" }
                });
            }

            if (ken != null)
            {
                albumsToSeed.AddRange(new[]
                {
                    // Ken Carson Albums
                    new ArtistAlbum { Title = "Project X", ArtistId = ken.Id, ReleaseYear = 2021, Description = "Ken Carson's debut studio album, defining his signature energetic rage sound, heavy 808s, and fast-paced melodic delivery.", CoverImageUrl = "/images/albums/Ken_Carson/Project_X.jpg" },
                    new ArtistAlbum { Title = "XTENDED", ArtistId = ken.Id, ReleaseYear = 2022, Description = "The expanded deluxe edition of Ken Carson's second studio album X, adding five new high-energy tracks to his distorted trap sound.", CoverImageUrl = "/images/albums/Ken_Carson/XTENDED.jpg" },
                    new ArtistAlbum { Title = "A Great Chaos (Deluxe)", ArtistId = ken.Id, ReleaseYear = 2023, Description = "Ken Carson's breakthrough critically acclaimed album showcasing intense electronic rage production, distorted beats, and unhinged energy, expanded with deluxe material.", CoverImageUrl = "/images/albums/Ken_Carson/A_Great_Chaos_Deluxe.jpg" },
                    new ArtistAlbum { Title = "More Chaos", ArtistId = ken.Id, ReleaseYear = 2025, Description = "The high-voltage follow-up to A Great Chaos, amplifying aggressive distorted 808s, hypnotic synth hooks, and dark underground rage energy.", CoverImageUrl = "/images/albums/Ken_Carson/More_Chaos.jpg" },
                    new ArtistAlbum { Title = "xperiment - cartunez", ArtistId = ken.Id, ReleaseYear = 2026, Description = "A massive two-disc opus uniting the boundary-pushing electronic experimentation of Xperiment with the aggressive Cartunez addendum.", CoverImageUrl = "/images/albums/Ken_Carson/Xperiment_Cartunez.jpg" }
                });
            }

            if (don != null)
            {
                albumsToSeed.AddRange(new[]
                {
                    // Don Toliver Albums
                    new ArtistAlbum { Title = "Donny Womack", ArtistId = don.Id, ReleaseYear = 2018, Description = "Don Toliver's breakthrough mixtape that showcased his unique melodic rap flow.", CoverImageUrl = "/images/albums/Don_Toliver/Donny_Womack.jpg" },
                    new ArtistAlbum { Title = "Heaven Or Hell", ArtistId = don.Id, ReleaseYear = 2020, Description = "Don Toliver's debut studio album, featuring his signature melodic trap sound and the hit single 'After Party'.", CoverImageUrl = "/images/albums/Don_Toliver/Heaven_Or_Hell.jpg" },
                    new ArtistAlbum { Title = "Life of a DON", ArtistId = don.Id, ReleaseYear = 2021, Description = "A highly anticipated sophomore album blending atmospheric R&B with Houston trap influences.", CoverImageUrl = "/images/albums/Don_Toliver/Life_of_a_DON.jpg" },
                    new ArtistAlbum { Title = "Love Sick (Deluxe)", ArtistId = don.Id, ReleaseYear = 2023, Description = "A conceptually driven album exploring themes of love and heartbreak with an eclectic, groovy production style.", CoverImageUrl = "/images/albums/Don_Toliver/Love_Sick_Deluxe.jpg" },
                    new ArtistAlbum { Title = "HARDSTONE PSYCHO", ArtistId = don.Id, ReleaseYear = 2024, Description = "Don Toliver's biker-themed album, delivering high-energy trap anthems and deep atmospheric cuts.", CoverImageUrl = "/images/albums/Don_Toliver/Hardstone_Psycho.jpg" },
                    new ArtistAlbum { Title = "OCTANE", ArtistId = don.Id, ReleaseYear = 2026, Description = "Don Toliver's fifth studio album leaning fully into the psychedelic trap-R&B sound he's been refining for years.", CoverImageUrl = "/images/albums/Don_Toliver/Octane.jpg" }
                });
            }

            var existingAlbums = await db.ArtistAlbums
                .Select(a => new { a.ArtistId, a.Title })
                .ToListAsync();
            var existingAlbumSet = new HashSet<(int ArtistId, string Title)>(
                existingAlbums.Select(a => (a.ArtistId, a.Title))
            );

            var newAlbums = albumsToSeed
                .Where(a => !existingAlbumSet.Contains((a.ArtistId, a.Title)))
                .ToList();

            if (newAlbums.Any())
            {
                db.ArtistAlbums.AddRange(newAlbums);
                await db.SaveChangesAsync();
            }

            await UpdateAlbumCoversAsync(db);
        }

        private static async Task SeedAlbumTracksAsync(AppDbContext db)
        {
            var albums = await db.ArtistAlbums.ToListAsync();

            var albumMap = albums.ToDictionary(a => a.Title, a => a.Id);
            var existingTracks = await db.AlbumTracks.ToListAsync();
            var existingTrackMap = existingTracks
                .ToDictionary(t => (t.ArtistAlbumId, t.TrackNumber));

            var albumsToSync = new HashSet<string>
            {
                "Project X",
                "XTENDED",
                "A Great Chaos (Deluxe)",
                "More Chaos",
                "xperiment - cartunez",
                "Donny Womack",
                "Heaven Or Hell",
                "Life of a DON",
                "Love Sick (Deluxe)",
                "HARDSTONE PSYCHO",
                "OCTANE"
            };

            void AddTracks(string albumTitle, params (int Number, string Name, bool IsTop, int DurationSeconds)[] tracks)
            {
                if (!albumMap.ContainsKey(albumTitle))
                {
                    return;
                }

                var albumId = albumMap[albumTitle];
                var sync = albumsToSync.Contains(albumTitle);

                foreach (var track in tracks)
                {
                    if (existingTrackMap.TryGetValue((albumId, track.Number), out var existing))
                    {
                        if (sync)
                        {
                            existing.Name = track.Name;
                            existing.IsTopTrack = track.IsTop;
                            existing.DurationSeconds = track.DurationSeconds;
                        }
                        continue;
                    }

                    var created = new AlbumTrack
                    {
                        ArtistAlbumId = albumId,
                        TrackNumber = track.Number,
                        Name = track.Name,
                        IsTopTrack = track.IsTop,
                        DurationSeconds = track.DurationSeconds
                    };

                    db.AlbumTracks.Add(created);
                    existingTrackMap[(albumId, track.Number)] = created;
                }
            }

            // Kanye West tracklists
            AddTracks("The College Dropout",
                (1, "Intro", false, 19),
                (2, "We Don't Care", false, 239),
                (3, "Graduation Day", false, 81),
                (4, "All Falls Down", true, 223),
                (5, "I'll Fly Away", false, 69),
                (6, "Spaceship", false, 324),
                (7, "Jesus Walks", true, 193),
                (8, "Never Let Me Down", false, 324),
                (9, "Get Em High", false, 289),
                (10, "Workout Plan", false, 46),
                (11, "The New Workout Plan", false, 322),
                (12, "Slow Jamz", false, 316),
                (13, "Breathe In Breathe Out", false, 246),
                (14, "School Spirit Skit 1", false, 78),
                (15, "School Spirit", false, 182),
                (16, "School Spirit Skit 2", false, 43),
                (17, "Lil Jimmy Skit", false, 53),
                (18, "Two Words", false, 266),
                (19, "Through the Wire", true, 221),
                (20, "Family Business", false, 278),
                (21, "Last Call", false, 760)
            );

            AddTracks("Late Registration",
                (1, "Wake Up Mr. West", false, 41),
                (2, "Heard 'Em Say", true, 203),
                (3, "Touch the Sky", true, 236),
                (4, "Gold Digger", true, 207),
                (5, "Skit #1", false, 33),
                (6, "Drive Slow", false, 272),
                (7, "My Way Home", false, 103),
                (8, "Crack Music", false, 270),
                (9, "Roses", false, 245),
                (10, "Bring Me Down", false, 198),
                (11, "Addiction", false, 267),
                (12, "Skit #2", false, 31),
                (13, "Diamonds from Sierra Leone Remix", false, 233),
                (14, "We Major", false, 448),
                (15, "Skit #3", false, 24),
                (16, "Hey Mama", false, 305),
                (17, "Celebration", false, 198),
                (18, "Skit #4", false, 78),
                (19, "Gone", false, 333),
                (20, "Diamonds from Sierra Leone", false, 238),
                (21, "Late", false, 230)
            );

            AddTracks("Graduation",
                (1, "Good Morning", false, 195),
                (2, "Champion", false, 167),
                (3, "Stronger", true, 311),
                (4, "I Wonder", true, 243),
                (5, "Good Life", false, 207),
                (6, "Can't Tell Me Nothing", false, 271),
                (7, "Barry Bonds", false, 204),
                (8, "Drunk and Hot Girls", false, 313),
                (9, "Flashing Lights", true, 237),
                (10, "Everything I Am", false, 227),
                (11, "The Glory", false, 212),
                (12, "Homecoming", false, 203),
                (13, "Big Brother", false, 287)
            );

            AddTracks("808s & Heartbreak",
                (1, "Say You Will", false, 377),
                (2, "Welcome to Heartbreak", false, 262),
                (3, "Heartless", true, 211),
                (4, "Amazing", true, 238),
                (5, "Love Lockdown", true, 270),
                (6, "Paranoid", false, 277),
                (7, "RoboCop", false, 274),
                (8, "Street Lights", false, 189),
                (9, "Bad News", false, 238),
                (10, "See You In My Nightmares", false, 258),
                (11, "Coldest Winter", false, 164),
                (12, "Pinnocchio Story", false, 361)
            );

            AddTracks("My Beautiful Dark Twisted Fantasy",
                (1, "Dark Fantasy", false, 280),
                (2, "Gorgeous", false, 357),
                (3, "Power", true, 292),
                (4, "All of the Lights (Interlude)", false, 62),
                (5, "All of the Lights", true, 299),
                (6, "Monster", false, 378),
                (7, "So Appalled", false, 398),
                (8, "Devil in a New Dress", false, 352),
                (9, "Runaway", true, 548),
                (10, "Hell of a Life", false, 327),
                (11, "Blame Game", false, 469),
                (12, "Lost in the World", false, 256),
                (13, "Who Will Survive in America", false, 98)
            );

            AddTracks("Yeezus",
                (1, "On Sight", true, 156),
                (2, "Black Skinhead", true, 188),
                (3, "I Am a God", false, 231),
                (4, "New Slaves", false, 256),
                (5, "Hold My Liquor", false, 326),
                (6, "I'm in It", false, 234),
                (7, "Blood on the Leaves", false, 360),
                (8, "Guilt Trip", false, 243),
                (9, "Send It Up", false, 178),
                (10, "Bound 2", true, 229)
            );

            AddTracks("The Life of Pablo",
                (1, "Ultralight Beam", false, 320),
                (2, "Father Stretch My Hands, Pt. 1", true, 135),
                (3, "Pt. 2", false, 130),
                (4, "Famous", true, 196),
                (5, "Feedback", false, 147),
                (6, "Low Lights", false, 131),
                (7, "Highlights", false, 199),
                (8, "Freestyle 4", false, 123),
                (9, "I Love Kanye", false, 44),
                (10, "Waves", true, 181),
                (11, "FML", false, 236),
                (12, "Real Friends", false, 251),
                (13, "Wolves", false, 301),
                (14, "Frank's Track", false, 38),
                (15, "Siiiiiiiiilver Surffffeeeeer Intermission", false, 56),
                (16, "30 Hours", false, 323),
                (17, "No More Parties in LA", false, 374),
                (18, "Facts", false, 200),
                (19, "Fade", false, 193),
                (20, "Saint Pablo", false, 372)
            );

            AddTracks("Ye",
                (1, "I Thought About Killing You", false, 274),
                (2, "Yikes", false, 188),
                (3, "All Mine", true, 145),
                (4, "Wouldn't Leave", false, 205),
                (5, "No Mistakes", false, 123),
                (6, "Ghost Town", true, 271),
                (7, "Violent Crimes", true, 215)
            );

            AddTracks("Jesus Is King",
                (1, "Every Hour", false, 112),
                (2, "Selah", true, 164),
                (3, "Follow God", true, 104),
                (4, "Closed on Sunday", true, 151),
                (5, "On God", false, 136),
                (6, "Everything We Need", false, 116),
                (7, "Water", false, 168),
                (8, "God Is", false, 203),
                (9, "Hands On", false, 203),
                (10, "Use This Gospel", false, 213),
                (11, "Jesus Is Lord", false, 49)
            );

            AddTracks("Donda",
                (1, "Donda Chant", false, 52),
                (2, "Jail", false, 297),
                (3, "God Breathed", false, 333),
                (4, "Off the Grid", false, 339),
                (5, "Hurricane", true, 243),
                (6, "Praise God", true, 226),
                (7, "Jonah", false, 195),
                (8, "Ok Ok", false, 204),
                (9, "Junya", false, 147),
                (10, "Believe What I Say", false, 242),
                (11, "24", false, 197),
                (12, "Remote Control", false, 198),
                (13, "Moon", true, 156),
                (14, "Heaven and Hell", false, 145),
                (15, "Donda", false, 128),
                (16, "Keep My Spirit Alive", false, 221),
                (17, "Jesus Lord", false, 538),
                (18, "New Again", false, 183),
                (19, "Tell The Vision", false, 104),
                (20, "Lord I Need You", false, 162),
                (21, "Pure Souls", false, 358),
                (22, "Come to Life", false, 310),
                (23, "No Child Left Behind", false, 178),
                (24, "Jail, Pt. 2", false, 297),
                (25, "Ok Ok, Pt. 2", false, 204),
                (26, "Junya, Pt. 2", false, 182),
                (27, "Jesus Lord, Pt. 2", false, 690)
            );

            AddTracks("Donda 2",
                (1, "True Love", true, 197),
                (2, "Broken Road", true, 117),
                (3, "Get Lost", false, 155),
                (4, "Too Easy", false, 178),
                (5, "Flowers", false, 171),
                (6, "Security", false, 136),
                (7, "We Did It Kid", false, 168),
                (8, "Pablo", false, 154),
                (9, "Louie Bags", false, 193),
                (10, "Happy", false, 285),
                (11, "Sci-Fi", false, 240),
                (12, "Selfish", false, 99),
                (13, "Lord Lift Me Up", false, 129),
                (14, "City of Gods", false, 256),
                (15, "First Time in a Long Time", false, 184),
                (16, "Eazy", false, 234),
                (17, "Mr. Miyagi", false, 181),
                (18, "Burn Everything", false, 141),
                (19, "Maintenance", false, 186),
                (20, "530", true, 290),
                (21, "Candyland", false, 100),
                (22, "Suzy", false, 161),
                (23, "Nebraska", false, 210),
                (24, "Closed for Business", false, 134),
                (25, "Stop Looking for This", false, 174)
            );

            AddTracks("Bully",
                (1, "King", true, 203),
                (2, "This a Must", false, 236),
                (3, "Father", true, 207),
                (4, "All the Love", true, 272),
                (5, "Punch Drunk", false, 103),
                (6, "Whatever Works", false, 270),
                (7, "Mama's Favorite", false, 245),
                (8, "Sisters and Brothers", false, 198),
                (9, "Bully", false, 267),
                (10, "Highs and Lows", false, 233),
                (11, "I Can't Wait", false, 448),
                (12, "White Lines", false, 305),
                (13, "Circles", false, 198),
                (14, "Preacher Man", false, 333),
                (15, "Beauty and the Beast", false, 238),
                (16, "Damn", false, 230),
                (17, "Last Breath", false, 252),
                (18, "This One Here", false, 225)
            );

            // Nirvana tracklists
            AddTracks("Bleach",
                (1, "Blew", true, 175),
                (2, "Floyd the Barber", false, 138),
                (3, "About a Girl", true, 168),
                (4, "School", false, 162),
                (5, "Love Buzz", true, 215),
                (6, "Paper Cuts", false, 246),
                (7, "Negative Creep", false, 176),
                (8, "Scoff", false, 250),
                (9, "Swap Meet", false, 183),
                (10, "Mr. Moustache", false, 203),
                (11, "Sifting", false, 322),
                (12, "Big Cheese", false, 222),
                (13, "Downer", false, 103)
            );

            AddTracks("Nevermind",
                (1, "Smells Like Teen Spirit", true, 301),
                (2, "In Bloom", true, 255),
                (3, "Come as You Are", true, 219),
                (4, "Breed", false, 184),
                (5, "Lithium", false, 257),
                (6, "Polly", false, 177),
                (7, "Territorial Pissings", false, 142),
                (8, "Drain You", false, 223),
                (9, "Lounge Act", false, 156),
                (10, "Stay Away", false, 211),
                (11, "On a Plain", false, 196),
                (12, "Something in the Way", false, 231)
            );

            AddTracks("In Utero",
                (1, "Serve the Servants", false, 216),
                (2, "Scentless Apprentice", false, 228),
                (3, "Heart-Shaped Box", true, 281),
                (4, "Rape Me", true, 169),
                (5, "Frances Farmer Will Have Her Revenge on Seattle", false, 249),
                (6, "Dumb", true, 152),
                (7, "Very Ape", false, 116),
                (8, "Milk It", false, 235),
                (9, "Pennyroyal Tea", false, 217),
                (10, "Radio Friendly Unit Shifter", false, 291),
                (11, "tourette's", false, 95),
                (12, "All Apologies", false, 231)
            );

            // Kendrick Lamar tracklists
            AddTracks("Overly Dedicated",
                (1, "The Heart Pt. 2", false, 295),
                (2, "Growing Apart (To Get Closer)", false, 221),
                (3, "Night of the Living Junkies", false, 212),
                (4, "P&P 1.5", false, 362),
                (5, "Alien Girl (Today W/ Her)", false, 248),
                (6, "Opposites Attract (Tomorrow W/O Her)", true, 272),
                (7, "Michael Jordan", true, 351),
                (8, "Ignorance Is Bliss", true, 208),
                (9, "R.O.T.C. (Interlude)", false, 163),
                (10, "Barbed Wire", false, 266),
                (11, "Average Joe", false, 256),
                (12, "H.O.C", false, 317),
                (13, "Cut You Off (To Grow Closer)", false, 364),
                (14, "Heaven & Hell", false, 203),
                (15, "She Needs Me (Remix)", false, 196),
                (16, "I Do This (Remix)", false, 243)
            );

            AddTracks("Section.80",
                (1, "Fuck Your Ethnicity", false, 224),
                (2, "Hol' Up", false, 173),
                (3, "A.D.H.D", true, 215),
                (4, "No Make-Up (Her Vice)", false, 235),
                (5, "Tammy's Song (Her Evils)", false, 161),
                (6, "Chapter Six", false, 161),
                (7, "Ronald Reagan Era", false, 216),
                (8, "Poe Mans Dreams (His Vice)", false, 261),
                (9, "The Spiteful Chant", false, 320),
                (10, "Chapter Ten", false, 75),
                (11, "Keisha's Song (Her Pain)", false, 227),
                (12, "Rigamortus", true, 168),
                (13, "Kush & Corinthians", false, 304),
                (14, "Blow My High (Members Only)", false, 215),
                (15, "Ab-Soul's Outro", false, 350),
                (16, "HiiiPoWeR", true, 279)
            );

            AddTracks("good kid, m.A.A.d city (deluxe)",
                (1, "Sherane a.k.a Master Splinter's Daughter", false, 273),
                (2, "Bitch, Don't Kill My Vibe", true, 310),
                (3, "Backseat Freestyle", false, 212),
                (4, "The Art of Peer Pressure", false, 324),
                (5, "Money Trees", true, 386),
                (6, "Poetic Justice", false, 300),
                (7, "good kid", false, 214),
                (8, "m.A.A.d city", false, 350),
                (9, "Swimming Pools (Drank)", true, 313),
                (10, "Sing About Me, I'm Dying of Thirst", false, 723),
                (11, "Real", false, 443),
                (12, "Compton", false, 248),
                (13, "The Recipe", false, 352),
                (14, "Black Boy Fly", false, 278),
                (15, "Now or Never", false, 257),
                (16, "Bitch, Don't Kill My Vibe (Remix)", false, 278)
            );

            AddTracks("To Pimp a Butterfly",
                (1, "Wesley's Theory", false, 287),
                (2, "For Free? (Interlude)", false, 130),
                (3, "King Kunta", true, 234),
                (4, "Institutionalized", false, 271),
                (5, "These Walls", false, 300),
                (6, "u", false, 268),
                (7, "Alright", true, 219),
                (8, "For Sale? (Interlude)", false, 291),
                (9, "Momma", false, 283),
                (10, "Hood Politics", false, 283),
                (11, "How Much a Dollar Cost", false, 261),
                (12, "Complexion (A Zulu Love)", false, 263),
                (13, "The Blacker the Berry", true, 328),
                (14, "You Ain't Gotta Lie (Momma Said)", false, 241),
                (15, "i", false, 336),
                (16, "Mortal Man", false, 727)
            );

            AddTracks("untitled unmastered.",
                (1, "untitled 01 | 08.19.2014.", false, 247),
                (2, "untitled 02 | 06.23.2014.", true, 258),
                (3, "untitled 03 | 05.28.2013.", false, 154),
                (4, "untitled 04 | 08.14.2014.", false, 80),
                (5, "untitled 05 | 09.21.2014.", true, 338),
                (6, "untitled 06 | 06.30.2014.", false, 208),
                (7, "untitled 07 | 2014 - 2016", true, 496),
                (8, "untitled 08 | 09.06.2014.", false, 235)
            );

            AddTracks("DAMN.",
                (1, "BLOOD.", false, 118),
                (2, "DNA.", true, 185),
                (3, "YAH.", false, 160),
                (4, "ELEMENT.", false, 208),
                (5, "FEEL.", false, 214),
                (6, "LOYALTY.", false, 227),
                (7, "PRIDE.", false, 275),
                (8, "HUMBLE.", true, 177),
                (9, "LUST.", false, 307),
                (10, "LOVE.", true, 213),
                (11, "XXX.", false, 254),
                (12, "FEAR.", false, 460),
                (13, "GOD.", false, 248),
                (14, "DUCKWORTH.", false, 248)
            );

            AddTracks("Black Panther The Album Music From And Inspired By",
                (1, "Black Panther", false, 130),
                (2, "All The Stars", true, 232),
                (3, "X", false, 267),
                (4, "The Ways", false, 239),
                (5, "Opps", false, 180),
                (6, "I Am", false, 208),
                (7, "Paramedic!", false, 219),
                (8, "Bloody Waters", false, 272),
                (9, "King's Dead", true, 229),
                (10, "Redemption Interlude", false, 85),
                (11, "Redemption", false, 222),
                (12, "Seasons", false, 242),
                (13, "Big Shot", false, 221),
                (14, "Pray For Me", true, 211)
            );

            AddTracks("Mr. Morale & The Big Steppers",
                (1, "United in Grief", false, 255),
                (2, "N95", true, 195),
                (3, "Worldwide Steppers", false, 203),
                (4, "Die Hard", true, 239),
                (5, "Father Time", false, 222),
                (6, "Rich (Interlude)", false, 103),
                (7, "Rich Spirit", false, 202),
                (8, "We Cry Together", false, 341),
                (9, "Purple Hearts", false, 329),
                (10, "Count Me Out", false, 283),
                (11, "Crown", false, 264),
                (12, "Silent Hill", true, 220),
                (13, "Savior (Interlude)", false, 152),
                (14, "Savior", false, 224),
                (15, "Auntie Diaries", false, 281),
                (16, "Mr. Morale", false, 210),
                (17, "Mother I Sober", false, 406),
                (18, "Mirror", false, 256)
            );

            AddTracks("GNX",
                (1, "wacced out murals", false, 334),
                (2, "squabble up", true, 157),
                (3, "luther", true, 177),
                (4, "man at the garden", false, 232),
                (5, "heydude", false, 172),
                (6, "reincarnated", false, 275),
                (7, "tv off", true, 142),
                (8, "dodger blue", false, 131),
                (9, "peekaboo", false, 154),
                (10, "heart pt. 6", false, 290),
                (11, "gnx", false, 201),
                (12, "gloria", false, 287)
            );

            // Ken Carson tracklists
            AddTracks("Project X",
                (1, "Who's Next", false, 104),
                (2, "Rock N Roll", true, 148),
                (3, "Party All Day", false, 150),
                (4, "Change", false, 160),
                (5, "Run + Ran", true, 244),
                (6, "Shake", false, 126),
                (7, "Hella", true, 161),
                (8, "Clutch", false, 131),
                (9, "Till I Die", false, 129),
                (10, "Burnin Up", false, 154),
                (11, "So What", false, 133)
            );

            AddTracks("XTENDED",
                (1, "Intro", false, 195),
                (2, "New", false, 169),
                (3, "Gems", false, 136),
                (4, "Nobody", false, 130),
                (5, "Go", false, 110),
                (6, "MDMA", true, 228),
                (7, "X", false, 129),
                (8, "PDBMH", false, 138),
                (9, "Money Hunt", false, 105),
                (10, "South Beach", false, 120),
                (11, "Going Schitz", false, 115),
                (12, "Same Thing", false, 115),
                (13, "Freestyle 1", false, 231),
                (14, "Freestyle 2", true, 138),
                (15, "Fuk 12", false, 84),
                (16, "Murda Musik", true, 176),
                (17, "Delinquent", false, 165),
                (18, "Get Rich Or Die", false, 139),
                (19, "Turn Up", false, 118),
                (20, "The End", false, 135),
                (21, "Freestyle 3", true, 142),
                (22, "Fashion Habits", false, 201),
                (23, "Shoot", false, 166),
                (24, "Swag Overload", false, 152),
                (25, "Lookbook", false, 150)
            );

            AddTracks("A Great Chaos (Deluxe)",
                (1, "Green Room", false, 188),
                (2, "Jennifer's Body", false, 158),
                (3, "Fighting My Demons", true, 150),
                (4, "Singapore", false, 151),
                (5, "Lose It", false, 140),
                (6, "Hardcore", false, 124),
                (7, "Me N My Kup", false, 234),
                (8, "It's Over", false, 95),
                (9, "Succubus", false, 149),
                (10, "Paranoid", false, 127),
                (11, "Pots", false, 129),
                (12, "Like This", false, 192),
                (13, "Overtime", false, 106),
                (14, "Vampire Hour", false, 154),
                (15, "Nightcore", false, 187),
                (16, "Nightcore 2", false, 182),
                (17, "Rockstar Lifestyle", false, 192),
                (18, "i need u", false, 148),
                (19, "loading", false, 186),
                (20, "more chaos", false, 141),
                (21, "toxic", false, 165),
                (22, "leather jacket", false, 157),
                (23, "mewtwo", false, 128),
                (24, "ss", true, 184),
                (25, "overseas", true, 141)
            );

            AddTracks("More Chaos",
                (1, "Lord Of Chaos", false, 121),
                (2, "Xposed", false, 230),
                (3, "Money Spread", false, 105),
                (4, "Root Of All Evil", false, 147),
                (5, "K-Hole", false, 198),
                (6, "Trap Jump", true, 150),
                (7, "Blakk Rokkstar", true, 230),
                (8, "LiveLeak", false, 190),
                (9, "Diamonds", false, 229),
                (10, "Dismantled", false, 144),
                (11, "200 Kash", false, 104),
                (12, "Down2Earth", false, 122),
                (13, "Confetti", false, 187),
                (14, "Naked", false, 165),
                (15, "Kryptonite", false, 167),
                (16, "Psycho", false, 138),
                (17, "Inferno", false, 156),
                (18, "Thx", false, 172),
                (19, "2000", false, 156),
                (20, "Evolution", false, 193),
                (21, "Ghoul", false, 169),
                (22, "Off The Meter", true, 219)
            );

            AddTracks("xperiment - cartunez",
                (1, "wheredoistart", false, 163),
                (2, "deaf note", true, 198),
                (3, "shadeson", true, 118),
                (4, "gynecologist", true, 135),
                (5, "wrist", false, 170),
                (6, "edm", false, 208),
                (7, "truth", false, 146),
                (8, "outofmybody", false, 121),
                (9, "the ritual", false, 160),
                (10, "interlude", false, 88),
                (11, "ghost", false, 233),
                (12, "drug kit", false, 207),
                (13, "possession", false, 133),
                (14, "fw00", false, 162),
                (15, "somanybags", false, 109),
                (16, "shopping", false, 239),
                (17, "amandabynes", false, 150),
                (18, "amnesia", false, 140),
                (19, "flamethrower", false, 139),
                (20, "knocking", false, 195),
                (21, "addiction", false, 196),
                (22, "wedidit", false, 222),
                (23, "scrape", false, 148),
                (24, "swag jack", false, 137),
                (25, "off a bean", false, 130),
                (26, "rare creature", false, 143),
                (27, "50k", false, 145),
                (28, "roxy reynolds", false, 160),
                (29, "005", false, 200),
                (30, "eaters", false, 128),
                (31, "9 bitches", false, 178),
                (32, "vamp city", false, 110),
                (33, "fake news", false, 123),
                (34, "disturbing the peace", false, 133),
                (35, "yvngvmp", false, 180)
            );

            // Don Toliver tracklists
            AddTracks("Donny Womack",
                (1, "Diamonds", false, 144),
                (2, "Diva", true, 201),
                (3, "Issues", false, 187),
                (4, "Backend", true, 231),
                (5, "Mamma Mia", false, 171),
                (6, "Holdin' Steel", false, 187),
                (7, "Bang Bang", false, 166),
                (8, "2 Lil Shorty", false, 149),
                (9, "Video Girl", true, 156),
                (10, "Around", false, 168),
                (11, "AMG", false, 167),
                (12, "Run Up", false, 181),
                (13, "Talk No More", false, 210)
            );

            AddTracks("Heaven Or Hell",
                (1, "Heaven Or Hell", false, 161),
                (2, "Euphoria", false, 214),
                (3, "Cardigan", true, 160),
                (4, "After Party", true, 167),
                (5, "Wasted", false, 168),
                (6, "Can't Feel My Legs", false, 180),
                (7, "Candy", false, 203),
                (8, "Company", false, 212),
                (9, "Had Enough", false, 157),
                (10, "Spaceship", false, 180),
                (11, "No Idea", true, 154),
                (12, "No Photos", false, 182)
            );

            AddTracks("Life of a DON",
                (1, "Xscape", false, 156),
                (2, "5x", false, 132),
                (3, "Way Bigger", false, 196),
                (4, "Flocky Flocky (feat. Travis Scott)", true, 183),
                (5, "What You Need", false, 218),
                (6, "Double Standards", false, 191),
                (7, "Swangin' on Westheimer", false, 288),
                (8, "Drugs N Hella Melodies (feat. Kali Uchis)", true, 199),
                (9, "2AM", false, 165),
                (10, "Get Throwed", false, 155),
                (11, "Company, Pt 2", false, 211),
                (12, "Outerspace (feat. Baby Keem)", false, 226),
                (13, "Smoke (feat. HVN and SoFaygo)", false, 199),
                (14, "You (feat. Travis Scott)", true, 213),
                (15, "Crossfaded", false, 159),
                (16, "Bogus", false, 199)
            );

            AddTracks("Love Sick (Deluxe)",
                (1, "No Pole", true, 187),
                (2, "Embarrassed (feat. Travis Scott)", true, 192),
                (3, "Geronimo", false, 123),
                (4, "Luckily I'm Having (feat. Teezo Touchdown)", false, 194),
                (5, "LoveSickness", false, 114),
                (6, "Let Her Go (feat. James Blake)", false, 192),
                (7, "Leave the Club (feat. Lil Durk & GloRilla)", false, 222),
                (8, "4 Me (feat. Kali Uchis)", false, 160),
                (9, "Go Down (feat. TisaKorean)", false, 210),
                (10, "Time Heals All", false, 233),
                (11, "Leather Coat", false, 241),
                (12, "Honeymoon", false, 184),
                (13, "Private Landing (feat. Justin Bieber & Future)", true, 238),
                (14, "Slow Motion (feat. Wizkid)", false, 168),
                (15, "Do It Right", false, 174),
                (16, "If I Had (feat. Charlie Wilson)", false, 208),
                (17, "Company Pt. 3", false, 180),
                (18, "Bus Stop (feat. Brent Faiyaz)", false, 236),
                (19, "Cinderella (feat. Toro y Moi)", false, 226),
                (20, "Encouragement", false, 176)
            );

            AddTracks("HARDSTONE PSYCHO",
                (1, "Kryptonite", false, 182),
                (2, "Tore Up", true, 126),
                (3, "Brother Stone", false, 202),
                (4, "Attitude", false, 161),
                (5, "Bandit", true, 147),
                (6, "Glock", false, 228),
                (7, "Ice Age", false, 218),
                (8, "4x4", false, 202),
                (9, "Purple Rain", false, 207),
                (10, "New Drop", true, 217),
                (11, "Backstreets", false, 197),
                (12, "Deep In The Water", false, 170),
                (13, "Inside", false, 250),
                (14, "5 To 10", false, 193),
                (15, "Last Laugh", false, 241),
                (16, "Hardstone National Anthem", false, 161),
                (17, "Rockstar Girl", false, 178),
                (18, "Love Is A Drug", false, 189),
                (19, "Donny Darko", false, 117),
                (20, "Geeked Up", false, 158)
            );

            AddTracks("OCTANE",
                (1, "E85", true, 153),
                (2, "Body", true, 155),
                (3, "Rendezvous (feat. Yeat)", false, 146),
                (4, "Secondhand (feat. Rema)", false, 226),
                (5, "Tiramisu", true, 138),
                (6, "ATM", false, 180),
                (7, "Long Way To Calabasas", false, 99),
                (8, "Rosary (feat. Travis Scott)", false, 194),
                (9, "All The Signs (feat. Teezo Touchdown)", false, 200),
                (10, "Call Back", false, 123),
                (11, "Tuition", false, 169),
                (12, "K9 (feat. SahBabii)", false, 154),
                (13, "Excavator", false, 211),
                (14, "Gemstone", false, 121),
                (15, "OPPOSITE", false, 157),
                (16, "TMU", false, 180),
                (17, "Pleasure's Mine", false, 177),
                (18, "Sweet Home", false, 200)
            );

            await db.SaveChangesAsync();
        }

        private static async Task UpdateAlbumCoversAsync(AppDbContext db)
        {
            var coverMap = new Dictionary<string, string>
            {
                // Kanye West
                { "The College Dropout", "/images/albums/Kanye_West/The_College_Dropout.jpg" },
                { "Late Registration", "/images/albums/Kanye_West/Late_Registration.jpg" },
                { "Graduation", "/images/albums/Kanye_West/Graduation.jpg" },
                { "808s & Heartbreak", "/images/albums/Kanye_West/808s_&_Heartbreak.jpg" },
                { "My Beautiful Dark Twisted Fantasy", "/images/albums/Kanye_West/My_Beautiful_Dark_Twisted_Fantasy.jpg" },
                { "Yeezus", "/images/albums/Kanye_West/Yeezus.jpg" },
                { "The Life of Pablo", "/images/albums/Kanye_West/The_Life_Of_Pablo.jpg" },
                { "Ye", "/images/albums/Kanye_West/Ye.jpg" },
                { "Jesus Is King", "/images/albums/Kanye_West/Jesus_Is_King.jpg" },
                { "Donda", "/images/albums/Kanye_West/Donda_With_Child.jpg" },
                { "Donda 2", "/images/albums/Kanye_West/Donda_2.jpg" },
                { "Bully", "/images/albums/Kanye_West/Bully.jpg" },

                // Sabrina Carpenter
                { "Eyes Wide Open", "/images/albums/Sabrina_Carpenter/Eyes_Wide_Open.jpg" },
                { "EVOLution", "/images/albums/Sabrina_Carpenter/EVOLution.jpg" },
                { "Singular: Act I", "/images/albums/Sabrina_Carpenter/Singular_Act1.jpg" },
                { "Singular: Act II", "/images/albums/Sabrina_Carpenter/Singular_Act2.jpg" },
                { "Emails I Can't Send", "/images/albums/Sabrina_Carpenter/Emails_I_Cant_Send.jpg" },
                { "Short n' Sweet", "/images/albums/Sabrina_Carpenter/Short_n_Sweet.jpg" },
                { "Man's Best Friend", "/images/albums/Sabrina_Carpenter/Mans_Best_Friend.jpg" },

                // Metallica
                { "Kill 'Em All", "/images/albums/Metallica/Kill_Em_All.jpg" },
                { "Ride the Lightning", "/images/albums/Metallica/Ride_The_Lightning.jpg" },
                { "Master of Puppets", "/images/albums/Metallica/Master_Of_Puppets.jpg" },
                { "...And Justice for All", "/images/albums/Metallica/And_Justice_For_All.jpg" },
                { "Metallica (The Black Album)", "/images/albums/Metallica/Metallica.jpg" },
                { "Load", "/images/albums/Metallica/Load.jpg" },
                { "Reload", "/images/albums/Metallica/Reload.jpg" },
                { "St. Anger", "/images/albums/Metallica/St_Anger.jpg" },
                { "Death Magnetic", "/images/albums/Metallica/Death_Magnetic.jpg" },
                { "Hardwired... to Self-Destruct", "/images/albums/Metallica/Hardwired_To_Self_Destruct.jpg" },
                { "72 Seasons", "/images/albums/Metallica/72_Seasons.jpg" },

                // Nirvana
                { "Bleach", "/images/albums/Nirvana/Bleach.jpg" },
                { "Nevermind", "/images/albums/Nirvana/Nevermind.jpg" },
                { "In Utero", "/images/albums/Nirvana/In_Utero.jpg" },

                // Kendrick Lamar
                { "Overly Dedicated", "/images/albums/Kendrick_Lamar/Overly_Dedicated.jpg" },
                { "Section.80", "/images/albums/Kendrick_Lamar/Section80.jpg" },
                { "good kid, m.A.A.d city (deluxe)", "/images/albums/Kendrick_Lamar/good_kid_maad_city_deluxe.jpg" },
                { "To Pimp a Butterfly", "/images/albums/Kendrick_Lamar/To_Pimp_A_Butterfly.jpg" },
                { "untitled unmastered.", "/images/albums/Kendrick_Lamar/untitled_unmastered.jpg" },
                { "DAMN.", "/images/albums/Kendrick_Lamar/DAMN.jpg" },
                { "Black Panther The Album Music From And Inspired By", "/images/albums/Kendrick_Lamar/Black_Panther_The_Album.jpg" },
                { "Mr. Morale & The Big Steppers", "/images/albums/Kendrick_Lamar/Mr_Morale_The_Big_Steppers.jpg" },
                { "GNX", "/images/albums/Kendrick_Lamar/GNX.jpg" },

                // Ken Carson
                { "Project X", "/images/albums/Ken_Carson/Project_X.jpg" },
                { "XTENDED", "/images/albums/Ken_Carson/XTENDED.jpg" },
                { "A Great Chaos (Deluxe)", "/images/albums/Ken_Carson/A_Great_Chaos_Deluxe.jpg" },
                { "More Chaos", "/images/albums/Ken_Carson/More_Chaos.jpg" },
                { "xperiment - cartunez", "/images/albums/Ken_Carson/Xperiment_Cartunez.jpg" },

                // Don Toliver
                { "Donny Womack", "/images/albums/Don_Toliver/Donny_Womack.jpg" },
                { "Heaven Or Hell", "/images/albums/Don_Toliver/Heaven_Or_Hell.jpg" },
                { "Life of a DON", "/images/albums/Don_Toliver/Life_of_a_DON.jpg" },
                { "Love Sick (Deluxe)", "/images/albums/Don_Toliver/Love_Sick.jpg" },
                { "HARDSTONE PSYCHO", "/images/albums/Don_Toliver/Hardstone_Psycho.jpg" },
                { "OCTANE", "/images/albums/Don_Toliver/Octane.jpg" }
            };

            foreach (var item in coverMap)
            {
                var album = await db.ArtistAlbums.FirstOrDefaultAsync(a => a.Title == item.Key);

                if (album != null)
                {
                    album.CoverImageUrl = item.Value;
                }
            }

            await db.SaveChangesAsync();
        }

        private static async Task SeedArtistMoodsAsync(AppDbContext db)
        {
            if (!await db.ArtistMoods.AnyAsync())
            {
                var kendrick = await db.Artists.FirstAsync(a => a.Name == "Kendrick Lamar");
                var kanye = await db.Artists.FirstAsync(a => a.Name == "Kanye West");
                var weeknd = await db.Artists.FirstAsync(a => a.Name == "The Weeknd");
                var tame = await db.Artists.FirstAsync(a => a.Name == "Tame Impala");
                var sabrina = await db.Artists.FirstAsync(a => a.Name == "Sabrina Carpenter");
                var nirvana = await db.Artists.FirstAsync(a => a.Name == "Nirvana");
                var travis = await db.Artists.FirstAsync(a => a.Name == "Travis Scott");
                var frank = await db.Artists.FirstAsync(a => a.Name == "Frank Ocean");
                var amy = await db.Artists.FirstAsync(a => a.Name == "Amy Winehouse");
                var daft = await db.Artists.FirstAsync(a => a.Name == "Daft Punk");
                var sleep = await db.Artists.FirstAsync(a => a.Name == "Sleep Token");
                var arctic = await db.Artists.FirstAsync(a => a.Name == "Arctic Monkeys");
                var metallica = await db.Artists.FirstAsync(a => a.Name == "Metallica");
                var queen = await db.Artists.FirstAsync(a => a.Name == "Queen");
                var tyler = await db.Artists.FirstAsync(a => a.Name == "Tyler, The Creator");

                var chill = await db.Moods.FirstAsync(m => m.Name == "Chill");
                var energetic = await db.Moods.FirstAsync(m => m.Name == "Energetic");
                var sad = await db.Moods.FirstAsync(m => m.Name == "Sad");
                var dark = await db.Moods.FirstAsync(m => m.Name == "Dark");
                var dreamy = await db.Moods.FirstAsync(m => m.Name == "Dreamy");
                var motivational = await db.Moods.FirstAsync(m => m.Name == "Motivational");
                var melancholic = await db.Moods.FirstAsync(m => m.Name == "Melancholic");
                var aggressive = await db.Moods.FirstAsync(m => m.Name == "Aggressive");
                var romantic = await db.Moods.FirstAsync(m => m.Name == "Romantic");
                var experimental = await db.Moods.FirstAsync(m => m.Name == "Experimental");

                db.ArtistMoods.AddRange(
                    new ArtistMood { ArtistId = kendrick.Id, MoodId = motivational.Id },
                    new ArtistMood { ArtistId = kendrick.Id, MoodId = energetic.Id },

                    new ArtistMood { ArtistId = kanye.Id, MoodId = experimental.Id },
                    new ArtistMood { ArtistId = kanye.Id, MoodId = motivational.Id },

                    new ArtistMood { ArtistId = weeknd.Id, MoodId = dark.Id },
                    new ArtistMood { ArtistId = weeknd.Id, MoodId = romantic.Id },

                    new ArtistMood { ArtistId = tame.Id, MoodId = dreamy.Id },
                    new ArtistMood { ArtistId = tame.Id, MoodId = chill.Id },

                    new ArtistMood { ArtistId = sabrina.Id, MoodId = romantic.Id },
                    new ArtistMood { ArtistId = sabrina.Id, MoodId = energetic.Id },

                    new ArtistMood { ArtistId = nirvana.Id, MoodId = aggressive.Id },
                    new ArtistMood { ArtistId = nirvana.Id, MoodId = melancholic.Id },

                    new ArtistMood { ArtistId = travis.Id, MoodId = energetic.Id },
                    new ArtistMood { ArtistId = travis.Id, MoodId = dark.Id },

                    new ArtistMood { ArtistId = frank.Id, MoodId = melancholic.Id },
                    new ArtistMood { ArtistId = frank.Id, MoodId = romantic.Id },

                    new ArtistMood { ArtistId = amy.Id, MoodId = sad.Id },
                    new ArtistMood { ArtistId = amy.Id, MoodId = melancholic.Id },

                    new ArtistMood { ArtistId = daft.Id, MoodId = energetic.Id },
                    new ArtistMood { ArtistId = daft.Id, MoodId = experimental.Id },

                    new ArtistMood { ArtistId = sleep.Id, MoodId = dark.Id },
                    new ArtistMood { ArtistId = sleep.Id, MoodId = experimental.Id },

                    new ArtistMood { ArtistId = arctic.Id, MoodId = chill.Id },
                    new ArtistMood { ArtistId = arctic.Id, MoodId = energetic.Id },

                    new ArtistMood { ArtistId = metallica.Id, MoodId = aggressive.Id },
                    new ArtistMood { ArtistId = metallica.Id, MoodId = energetic.Id },

                    new ArtistMood { ArtistId = queen.Id, MoodId = energetic.Id },
                    new ArtistMood { ArtistId = queen.Id, MoodId = romantic.Id },

                    new ArtistMood { ArtistId = tyler.Id, MoodId = experimental.Id },
                    new ArtistMood { ArtistId = tyler.Id, MoodId = chill.Id }
                );

                await db.SaveChangesAsync();
            }

            var ken = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Ken Carson");
            if (ken != null)
            {
                var targetMoodNames = new[] { "Aggressive", "Energetic", "Dark", "Experimental" };
                var moods = await db.Moods.Where(m => targetMoodNames.Contains(m.Name)).ToListAsync();
                var existingKenMoodIds = await db.ArtistMoods
                    .Where(am => am.ArtistId == ken.Id)
                    .Select(am => am.MoodId)
                    .ToListAsync();

                foreach (var mood in moods)
                {
                    if (!existingKenMoodIds.Contains(mood.Id))
                    {
                        db.ArtistMoods.Add(new ArtistMood
                        {
                            ArtistId = ken.Id,
                            MoodId = mood.Id
                        });
                    }
                }

                await db.SaveChangesAsync();
            }

            var don = await db.Artists.FirstOrDefaultAsync(a => a.Name == "Don Toliver");
            if (don != null)
            {
                var targetDonMoods = new[] { "Chill", "Energetic", "Dreamy" };
                var donMoods = await db.Moods.Where(m => targetDonMoods.Contains(m.Name)).ToListAsync();
                var existingDonMoodIds = await db.ArtistMoods
                    .Where(am => am.ArtistId == don.Id)
                    .Select(am => am.MoodId)
                    .ToListAsync();

                foreach (var mood in donMoods)
                {
                    if (!existingDonMoodIds.Contains(mood.Id))
                    {
                        db.ArtistMoods.Add(new ArtistMood
                        {
                            ArtistId = don.Id,
                            MoodId = mood.Id
                        });
                    }
                }

                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedLyricChallengesAsync(AppDbContext db)
        {
            var challenges = BuildLyricChallenges();
            var existing = await db.LyricChallenges.ToListAsync();

            foreach (var challenge in challenges)
            {
                var row = existing.FirstOrDefault(x =>
                    x.ArtistName == challenge.ArtistName && x.TrackName == challenge.TrackName);

                if (row == null)
                {
                    db.LyricChallenges.Add(challenge);
                    continue;
                }

                row.LyricsBefore = challenge.LyricsBefore;
                row.MissingWord = challenge.MissingWord;
                row.DistractorsJson = challenge.DistractorsJson;
                row.Hint = challenge.Hint;
                row.Difficulty = challenge.Difficulty;
            }

            await db.SaveChangesAsync();
        }

        private static List<LyricChallenge> BuildLyricChallenges()
        {
            return new List<LyricChallenge>
            {
                new LyricChallenge
                {
                    TrackName = "Stronger",
                    ArtistName = "Kanye West",
                    LyricsBefore = "Work it, make it, do it, makes us\nHarder, better, faster, stronger\nMore than ever, hour after hour",
                    MissingWord = "work is never over",
                    DistractorsJson = "[\"nothing lasts forever\", \"we can take it higher\", \"this is just the start\"]",
                    Hint = "The chorus ends with the idea that the work never stops.",
                    Difficulty = "Easy"
                },
                new LyricChallenge
                {
                    TrackName = "Heartless",
                    ArtistName = "Kanye West",
                    LyricsBefore = "How could you be so\nCold as the winter wind when it breeze yo",
                    MissingWord = "just remember that you talkin' to me though",
                    DistractorsJson = "[\"you need to watch the way you talking\", \"how could you be so cold to me now\", \"i never thought that you would let go\"]",
                    Hint = "The line reminds them who they are talking to.",
                    Difficulty = "Medium"
                },
                new LyricChallenge
                {
                    TrackName = "Gold Digger",
                    ArtistName = "Kanye West",
                    LyricsBefore = "Now I ain't sayin' she a gold digger",
                    MissingWord = "but she ain't messin' with no broke",
                    DistractorsJson = "[\"but she only dates the richest guys\", \"but she never looks at anyone poor\", \"but she wants a man with all the money\"]",
                    Hint = "The next line says she is not interested in someone with no money.",
                    Difficulty = "Medium"
                },
                new LyricChallenge
                {
                    TrackName = "Smells Like Teen Spirit",
                    ArtistName = "Nirvana",
                    LyricsBefore = "With the lights out, it's less dangerous",
                    MissingWord = "here we are now, entertain us",
                    DistractorsJson = "[\"here we are now, entertain me\", \"sing along now, follow us\", \"turn the lights on, entertain us\"]",
                    Hint = "The invitation in the chorus, in the plural.",
                    Difficulty = "Easy"
                },
                new LyricChallenge
                {
                    TrackName = "Come as You Are",
                    ArtistName = "Nirvana",
                    LyricsBefore = "Come as you are, as you were\nAs I want you to be",
                    MissingWord = "as a friend, as an old enemy",
                    DistractorsJson = "[\"as a friend, as a memory\", \"as a stranger, as a melody\", \"as you were, as you will be\"]",
                    Hint = "It goes from a friend to the opposite.",
                    Difficulty = "Medium"
                },
                new LyricChallenge
                {
                    TrackName = "Enter Sandman",
                    ArtistName = "Metallica",
                    LyricsBefore = "Exit light\nEnter night\nTake my hand",
                    MissingWord = "we're off to never-never land",
                    DistractorsJson = "[\"we're off to the end of the night\", \"we're falling into the sand\", \"we're heading into the dark\"]",
                    Hint = "The destination is the land from Peter Pan.",
                    Difficulty = "Easy"
                },
                new LyricChallenge
                {
                    TrackName = "Nothing Else Matters",
                    ArtistName = "Metallica",
                    LyricsBefore = "So close, no matter how far\nCouldn't be much more from the heart\nForever trusting who we are",
                    MissingWord = "and nothing else matters",
                    DistractorsJson = "[\"and nothing ever changes\", \"and nothing else happens\", \"and no one else matters\"]",
                    Hint = "The line repeats the title of the ballad.",
                    Difficulty = "Easy"
                },
                new LyricChallenge
                {
                    TrackName = "Master of Puppets",
                    ArtistName = "Metallica",
                    LyricsBefore = "Master of puppets, I'm pulling your strings\nTwisting your mind and smashing your dreams",
                    MissingWord = "blinded by me, you can't see a thing",
                    DistractorsJson = "[\"just call my name, I'll hear you scream\", \"caught in my web, you can't break free\", \"follow my voice, you can't turn away\"]",
                    Hint = "The next line says you cannot see a thing.",
                    Difficulty = "Hard"
                },
                new LyricChallenge
                {
                    TrackName = "Espresso",
                    ArtistName = "Sabrina Carpenter",
                    LyricsBefore = "Is it that sweet? I guess so\nSay you can't sleep, baby, I know",
                    MissingWord = "that's that me, espresso",
                    DistractorsJson = "[\"that's that me, cappuccino\", \"that's that sweet macchiato\", \"that's that you, americano\"]",
                    Hint = "The line ends with the drink from the title.",
                    Difficulty = "Easy"
                },
                new LyricChallenge
                {
                    TrackName = "Please Please Please",
                    ArtistName = "Sabrina Carpenter",
                    LyricsBefore = "Please, please, please\nDon't prove 'em right\nAnd please, please, please",
                    MissingWord = "don't bring me to tears",
                    DistractorsJson = "[\"don't bring me to fears\", \"don't leave me in tears\", \"don't prove them right again\"]",
                    Hint = "The last request is not to make her cry.",
                    Difficulty = "Easy"
                },
                new LyricChallenge
                {
                    TrackName = "Feather",
                    ArtistName = "Sabrina Carpenter",
                    LyricsBefore = "I feel so much lighter",
                    MissingWord = "like a feather with you off my mind",
                    DistractorsJson = "[\"like a feather with you off my chest\", \"like a feather with you out of my heart\", \"like a feather now that you are gone\"]",
                    Hint = "It compares the relief to a feather, once that person is gone.",
                    Difficulty = "Medium"
                },
                new LyricChallenge
                {
                    TrackName = "HUMBLE.",
                    ArtistName = "Kendrick Lamar",
                    LyricsBefore = "Be humble\nSit down",
                    MissingWord = "be humble",
                    DistractorsJson = "[\"stand up\", \"look around\", \"shut down\"]",
                    Hint = "The famous hook repeats the title of the track.",
                    Difficulty = "Easy"
                },
                new LyricChallenge
                {
                    TrackName = "Alright",
                    ArtistName = "Kendronic Lamar",
                    LyricsBefore = "Alls my life I has to fight, nigga\nHard times like, \"Yah!\"\nBad trips like, \"God!\"",
                    MissingWord = "we gon' be alright",
                    DistractorsJson = "[\"it's gonna be okay\", \"we will survive\", \"we're gonna make it\"]",
                    Hint = "The anthem's iconic uplifting chant in the chorus.",
                    Difficulty = "Easy"
                },
                new LyricChallenge
                {
                    TrackName = "DNA.",
                    ArtistName = "Kendrick Lamar",
                    LyricsBefore = "I got loyalty, got royalty inside my DNA\nCocaine quarter piece, got war and peace inside my DNA\nI got power, poison, pain and joy inside my DNA",
                    MissingWord = "I got hustle though, ambition inside my DNA",
                    DistractorsJson = "[\"I got anger, passion inside my DNA\", \"I got courage, love inside my DNA\", \"I got spirit, fire inside myDNA\"]",
                    Hint = "The line mentions hustle and ambition.",
                    Difficulty = "Medium"
                }
            };
        }
    }
}