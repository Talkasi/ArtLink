INSERT INTO "Techniques" ("Id", "Name", "Description")
VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Oil painting', 'Traditional oil painting technique');

INSERT INTO "Artists" ("Id", "PasswordHash", "Email", "FirstName", "LastName", "Bio", "Experience", "ProfilePicturePath")
VALUES (
           '11111111-1111-1111-1111-111111111111',
           'fake_hash_1',
           'artist@example.com',
           'Test',
           'Artist',
           'Bio of test artist',
           5,
           '/images/test_artist.jpg'
       );

INSERT INTO "Employers" ("Id", "CompanyName", "Email", "PasswordHash", "CpFirstName", "CpLastName")
VALUES (
           '44444444-4444-4444-4444-444444444444',
           'Test Employer Inc.',
           'employer@example.com',
           'fake_hash_employer',
           'ContactFirst',
           'ContactLast'
       );

INSERT INTO "Portfolios" ("Id", "ArtistId", "TechniqueId", "Title", "Description")
VALUES (
           '22222222-2222-2222-2222-222222222222',
           '11111111-1111-1111-1111-111111111111',
           'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
           'Test Portfolio',
           'Portfolio description (required)'
       );

INSERT INTO "Artworks" ("Id", "PortfolioId", "Title", "Description", "ImagePath")
VALUES (
           '33333333-3333-3333-3333-333333333333',
           '22222222-2222-2222-2222-222222222222',
           'Test Artwork',
           'Artwork description',
           '/images/test_artwork.jpg'
       );

INSERT INTO "Contracts" ("Id", "EmployerId", "ArtistId", "ProjectDescription", "StartDate", "EndDate", "Status")
VALUES (
           '55555555-5555-5555-5555-555555555555',
           '44444444-4444-4444-4444-444444444444',
           '11111111-1111-1111-1111-111111111111',
           'Test contract for integration tests',
           '2024-01-01 00:00:00',
           '2024-06-01 00:00:00',
           0
       );

INSERT INTO "Techniques" ("Id", "Name", "Description", "CreatedAt")
VALUES ('22222222-2222-2222-2222-222222222222', 'Oil Painting', 'Test technique', now());

