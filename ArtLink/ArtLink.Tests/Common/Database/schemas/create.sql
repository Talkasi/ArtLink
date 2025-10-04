CREATE TABLE "Techniques" (
                              "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
                              "Name" VARCHAR(100) NOT NULL,
                              "Description" VARCHAR(1000) NOT NULL,
                              "CreatedAt" TIMESTAMPTZ DEFAULT now()
);

CREATE TABLE "Artists" (
                           "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
                           "PasswordHash" VARCHAR(255) NOT NULL,
                           "Email" VARCHAR(255) NOT NULL,
                           "FirstName" VARCHAR(100) NOT NULL,
                           "LastName" VARCHAR(100) NOT NULL,
                           "Bio" VARCHAR(1000),
                           "Experience" INTEGER,
                           "ProfilePicturePath" VARCHAR(500),
                           "CreatedAt" TIMESTAMPTZ DEFAULT now()
);

CREATE TABLE "Employers" (
                             "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
                             "CompanyName" VARCHAR(255) NOT NULL,
                             "Email" VARCHAR(255) NOT NULL,
                             "PasswordHash" VARCHAR(255) NOT NULL,
                             "CpFirstName" VARCHAR(100) NOT NULL,
                             "CpLastName" VARCHAR(100) NOT NULL,
                             "CreatedAt" TIMESTAMPTZ DEFAULT now()
);

CREATE TABLE "Portfolios" (
                              "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
                              "ArtistId" UUID NOT NULL REFERENCES "Artists"("Id") ON DELETE CASCADE,
                              "TechniqueId" UUID NOT NULL REFERENCES "Techniques"("Id") ON DELETE CASCADE,
                              "Title" VARCHAR(200) NOT NULL,
                              "Description" VARCHAR(2000) NOT NULL,
                              "CreatedAt" TIMESTAMPTZ DEFAULT now()
);

CREATE TABLE "Artworks" (
                            "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
                            "PortfolioId" UUID NOT NULL REFERENCES "Portfolios"("Id") ON DELETE CASCADE,
                            "Title" VARCHAR(200) NOT NULL,
                            "Description" VARCHAR(2000),
                            "ImagePath" VARCHAR(500) NOT NULL,
                            "CreatedAt" TIMESTAMPTZ DEFAULT now()
);

CREATE TABLE "Contracts" (
                             "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
                             "ArtistId" UUID NOT NULL REFERENCES "Artists"("Id") ON DELETE CASCADE,
                             "EmployerId" UUID NOT NULL REFERENCES "Employers"("Id") ON DELETE CASCADE,
                             "ProjectDescription" VARCHAR(2000) NOT NULL,
                             "StartDate" TIMESTAMPTZ,
                             "EndDate" TIMESTAMPTZ,
                             "Status" INTEGER NOT NULL,
                             "CreatedAt" TIMESTAMPTZ DEFAULT now()
);
