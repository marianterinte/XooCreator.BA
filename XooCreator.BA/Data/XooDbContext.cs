using Microsoft.EntityFrameworkCore;

namespace XooCreator.BA.Data;

public class XooDbContext : DbContext
{
    public XooDbContext(DbContextOptions<XooDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<CreditWallet> CreditWallets => Set<CreditWallet>();
    public DbSet<CreditTransaction> CreditTransactions => Set<CreditTransaction>();
    public DbSet<Tree> Trees => Set<Tree>();
    public DbSet<TreeChoice> TreeChoices => Set<TreeChoice>();
    public DbSet<Creature> Creatures => Set<Creature>();
    public DbSet<Job> Jobs => Set<Job>();

    // Builder data
    public DbSet<BodyPart> BodyParts => Set<BodyPart>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<AnimalPartSupport> AnimalPartSupports => Set<AnimalPartSupport>();
    public DbSet<BuilderConfig> BuilderConfigs => Set<BuilderConfig>();
    public DbSet<Region> Regions => Set<Region>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Auth0Sub).IsUnique();
            e.Property(x => x.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<CreditWallet>(e =>
        {
            e.HasKey(x => x.UserId);
            e.HasOne(x => x.User).WithOne().HasForeignKey<CreditWallet>(x => x.UserId);
        });

        modelBuilder.Entity<CreditTransaction>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).IsRequired();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<Tree>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<TreeChoice>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tree).WithMany(x => x.Choices).HasForeignKey(x => x.TreeId);
            e.HasIndex(x => new { x.TreeId, x.Tier }).IsUnique();
        });

        modelBuilder.Entity<Creature>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Tree).WithMany().HasForeignKey(x => x.TreeId);
        });

        modelBuilder.Entity<Job>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).HasMaxLength(24);
            e.Property(x => x.Status).HasMaxLength(24);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        // Builder entities
        modelBuilder.Entity<BodyPart>(e =>
        {
            e.HasKey(x => x.Key);
            e.Property(x => x.Key).HasMaxLength(32);
            e.Property(x => x.Name).HasMaxLength(64).IsRequired();
            e.Property(x => x.Image).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<Region>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(64).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Animal>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Label).HasMaxLength(64).IsRequired();
            e.Property(x => x.Src).HasMaxLength(256).IsRequired();
            e.Property(x => x.IsHybrid).HasDefaultValue(false);
            e.HasOne(x => x.Region).WithMany(x => x.Animals).HasForeignKey(x => x.RegionId);
        });

        modelBuilder.Entity<AnimalPartSupport>(e =>
        {
            e.HasKey(x => new { x.AnimalId, x.PartKey });
            e.HasOne(x => x.Animal).WithMany(x => x.SupportedParts).HasForeignKey(x => x.AnimalId);
            e.HasOne(x => x.Part).WithMany().HasForeignKey(x => x.PartKey);
        });

        modelBuilder.Entity<BuilderConfig>(e =>
        {
            e.HasKey(x => x.Id);
        });

        // Seed builder data based on specs.txt
        // Parts
        var part_head = new BodyPart { Key = "head", Name = "Head", Image = "images/bodyparts/face.webp", IsBaseLocked = false };
        var part_body = new BodyPart { Key = "body", Name = "Body", Image = "images/bodyparts/body.webp", IsBaseLocked = false };
        var part_arms = new BodyPart { Key = "arms", Name = "Arms", Image = "images/bodyparts/hands.webp", IsBaseLocked = false };
        var part_legs = new BodyPart { Key = "legs", Name = "Legs", Image = "images/bodyparts/legs.webp", IsBaseLocked = true };
        var part_tail = new BodyPart { Key = "tail", Name = "Tail", Image = "images/bodyparts/tail.webp", IsBaseLocked = true };
        var part_wings = new BodyPart { Key = "wings", Name = "Wings", Image = "images/bodyparts/wings.webp", IsBaseLocked = true };
        var part_horn = new BodyPart { Key = "horn", Name = "Horn", Image = "images/bodyparts/horn.webp", IsBaseLocked = true };
        var part_horns = new BodyPart { Key = "horns", Name = "Horns", Image = "images/bodyparts/horns.webp", IsBaseLocked = true };

        modelBuilder.Entity<BodyPart>().HasData(part_head, part_body, part_arms, part_legs, part_tail, part_wings, part_horn, part_horns);

        // Regions (existing EN)
        var r_sahara    = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Sahara" };
        var r_jungle    = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Jungle" };
        var r_farm      = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Farm" };
        var r_savanna   = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Savanna" };
        var r_forest    = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "Forest" };
        var r_wetlands  = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Name = "Wetlands" };
        var r_mountains = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Name = "Mountains" };

        // Regions (RO)
        var rr_jungla   = new Region { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Name = "Jungl?" };
        var rr_savana   = new Region { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Name = "Savana" };
        var rr_stepa    = new Region { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Name = "Step?" };
        var rr_ferma    = new Region { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Name = "Ferm?" };

        modelBuilder.Entity<Region>().HasData(r_sahara, r_jungle, r_farm, r_savanna, r_forest, r_wetlands, r_mountains,
                                              rr_jungla, rr_savana, rr_stepa, rr_ferma);

        // Animals with valid deterministic GUIDs (existing EN)
        var a_bunny    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Label = "Bunny",    Src = "images/animals/base/bunny.jpg",    RegionId = r_farm.Id,      IsHybrid = false };
        var a_cat      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Label = "Cat",      Src = "images/animals/base/cat.jpg",      RegionId = r_farm.Id,      IsHybrid = false };
        var a_giraffe  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Label = "Giraffe",  Src = "images/animals/base/giraffe.jpg",  RegionId = r_savanna.Id,   IsHybrid = false };
        var a_dog      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Label = "Dog",      Src = "images/animals/base/dog.jpg",      RegionId = r_farm.Id,      IsHybrid = false };
        var a_fox      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Label = "Fox",      Src = "images/animals/base/fox.jpg",      RegionId = r_forest.Id,    IsHybrid = false };
        var a_hippo    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Label = "Hippo",    Src = "images/animals/base/hippo.jpg",    RegionId = r_wetlands.Id,  IsHybrid = false };
        var a_monkey   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), Label = "Monkey",   Src = "images/animals/base/monkey.jpg",   RegionId = r_jungle.Id,    IsHybrid = false };
        var a_camel    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), Label = "Camel",    Src = "images/animals/base/camel.jpg",    RegionId = r_sahara.Id,    IsHybrid = false };
        var a_deer     = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), Label = "Deer",     Src = "images/animals/base/deer.jpg",     RegionId = r_forest.Id,    IsHybrid = false };
        var a_duck     = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000a"), Label = "Duck",     Src = "images/animals/base/duck.jpg",     RegionId = r_wetlands.Id,  IsHybrid = false };
        var a_eagle    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000b"), Label = "Eagle",    Src = "images/animals/base/eagle.jpg",    RegionId = r_mountains.Id, IsHybrid = false };
        var a_elephant = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000c"), Label = "Elephant", Src = "images/animals/base/elephant.jpg", RegionId = r_savanna.Id,   IsHybrid = false };
        var a_ostrich  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000d"), Label = "Ostrich",  Src = "images/animals/base/ostrich.jpg",  RegionId = r_savanna.Id,   IsHybrid = false };
        var a_parrot   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000e"), Label = "Parrot",   Src = "images/animals/base/parrot.jpg",   RegionId = r_jungle.Id,    IsHybrid = false };

        // Animals (RO seeds)
        // Jungl?
        var ar_jaguar   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000f"), Label = "Jaguar",                 Src = "images/animals/base/jaguar.jpg",          RegionId = rr_jungla.Id, IsHybrid = false };
        var ar_tucan    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Label = "Tucan",                  Src = "images/animals/base/tucan.jpg",           RegionId = rr_jungla.Id, IsHybrid = false };
        var ar_anaconda = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Label = "Anaconda",               Src = "images/animals/base/anaconda.jpg",        RegionId = rr_jungla.Id, IsHybrid = false };
        var ar_capucin  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Label = "Maimu?a Capucin",        Src = "images/animals/base/maimuta_capucin.jpg", RegionId = rr_jungla.Id, IsHybrid = false };
        var ar_broasca  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Label = "Broasca Otr?vitoare",   Src = "images/animals/base/broasca_otravitoare.jpg", RegionId = rr_jungla.Id, IsHybrid = false };

        // Savana
        var ar_leu      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Label = "Leu",                    Src = "images/animals/base/leu.jpg",             RegionId = rr_savana.Id, IsHybrid = false };
        var ar_elefant  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000015"), Label = "Elefant African",        Src = "images/animals/base/elefant_african.jpg", RegionId = rr_savana.Id, IsHybrid = false };
        var ar_girafa   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000016"), Label = "Giraf?",                 Src = "images/animals/base/girafa.jpg",          RegionId = rr_savana.Id, IsHybrid = false };
        var ar_zebra    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000017"), Label = "Zebra",                  Src = "images/animals/base/zebra.jpg",           RegionId = rr_savana.Id, IsHybrid = false };
        var ar_rinocer  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000018"), Label = "Rinocer",                Src = "images/animals/base/rinocer.jpg",         RegionId = rr_savana.Id, IsHybrid = false };

        // Step?
        var ar_bizon    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000019"), Label = "Bizon (America de Nord)", Src = "images/animals/base/bizon.jpg",            RegionId = rr_stepa.Id,  IsHybrid = false };
        var ar_saiga    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001a"), Label = "Antilopa Saiga (Eurasia)", Src = "images/animals/base/saiga.jpg",            RegionId = rr_stepa.Id,  IsHybrid = false };
        var ar_lup      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001b"), Label = "Lup cenu?iu",            Src = "images/animals/base/lup_cenusiu.jpg",     RegionId = rr_stepa.Id,  IsHybrid = false };
        var ar_przew    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001c"), Label = "Cal s?lbatic (Przewalski)", Src = "images/animals/base/cal_przewalski.jpg",  RegionId = rr_stepa.Id,  IsHybrid = false };
        var ar_vultur   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001d"), Label = "Vultur de step?",        Src = "images/animals/base/vultur_de_stepa.jpg", RegionId = rr_stepa.Id,  IsHybrid = false };

        // Ferm?
        var ar_vaca     = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001e"), Label = "Vaca",                   Src = "images/animals/base/vaca.jpg",            RegionId = rr_ferma.Id,  IsHybrid = false };
        var ar_oaia     = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001f"), Label = "Oaia",                   Src = "images/animals/base/oaia.jpg",            RegionId = rr_ferma.Id,  IsHybrid = false };
        var ar_calul    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000020"), Label = "Calul",                   Src = "images/animals/base/calul.jpg",           RegionId = rr_ferma.Id,  IsHybrid = false };
        var ar_gaina    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000021"), Label = "G?ina",                   Src = "images/animals/base/gaina.jpg",           RegionId = rr_ferma.Id,  IsHybrid = false };
        var ar_porc     = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000022"), Label = "Porcul",                  Src = "images/animals/base/porc.jpg",            RegionId = rr_ferma.Id,  IsHybrid = false };

        modelBuilder.Entity<Animal>().HasData(
            a_bunny, a_cat, a_giraffe, a_dog, a_fox, a_hippo, a_monkey, a_camel, a_deer, a_duck, a_eagle, a_elephant, a_ostrich, a_parrot,
            ar_jaguar, ar_tucan, ar_anaconda, ar_capucin, ar_broasca,
            ar_leu, ar_elefant, ar_girafa, ar_zebra, ar_rinocer,
            ar_bizon, ar_saiga, ar_lup, ar_przew, ar_vultur,
            ar_vaca, ar_oaia, ar_calul, ar_gaina, ar_porc
        );

        // Helper to create supports
        AnimalPartSupport APS(Guid animal, string part) => new AnimalPartSupport { AnimalId = animal, PartKey = part };

        var baseParts = new[] { "head", "body", "arms" };

        var supports = new List<AnimalPartSupport>();
        void AddSupports(Guid animal, IEnumerable<string> parts)
        {
            foreach (var p in parts)
                supports.Add(APS(animal, p));
        }

        // existing EN
        AddSupports(a_bunny.Id, baseParts);
        AddSupports(a_cat.Id, baseParts);
        AddSupports(a_giraffe.Id, new[] { "head","body","arms","legs","tail","wings","horn","horns" });
        AddSupports(a_dog.Id, baseParts);
        AddSupports(a_fox.Id, baseParts);
        AddSupports(a_hippo.Id, baseParts);
        AddSupports(a_monkey.Id, baseParts);
        AddSupports(a_camel.Id, baseParts);
        AddSupports(a_deer.Id, new[] { "head","body","arms","legs","tail","wings","horn","horns" });
        AddSupports(a_duck.Id, new[] { "head","body","arms","legs","tail","wings" });
        AddSupports(a_eagle.Id, new[] { "head","body","arms","legs","tail","wings" });
        AddSupports(a_elephant.Id, baseParts);
        AddSupports(a_ostrich.Id, new[] { "head","body","arms","legs","tail","wings" });
        AddSupports(a_parrot.Id, new[] { "head","body","arms","legs","tail","wings" });

        // RO animals supports
        // Jungl?
        AddSupports(ar_jaguar.Id, baseParts);
        AddSupports(ar_tucan.Id, new[] { "head","body","arms","legs","tail","wings" });
        AddSupports(ar_anaconda.Id, new[] { "head","body","tail" });
        AddSupports(ar_capucin.Id, baseParts);
        AddSupports(ar_broasca.Id, new[] { "head","body","legs","tail" });

        // Savana
        AddSupports(ar_leu.Id, baseParts);
        AddSupports(ar_elefant.Id, baseParts);
        AddSupports(ar_girafa.Id, new[] { "head","body","arms","legs","tail" });
        AddSupports(ar_zebra.Id, new[] { "head","body","arms","legs","tail" });
        AddSupports(ar_rinocer.Id, new[] { "head","body","arms","legs","tail","horn" });

        // Step?
        AddSupports(ar_bizon.Id, baseParts);
        AddSupports(ar_saiga.Id, new[] { "head","body","arms","legs","tail","horns" });
        AddSupports(ar_lup.Id, baseParts);
        AddSupports(ar_przew.Id, new[] { "head","body","arms","legs","tail" });
        AddSupports(ar_vultur.Id, new[] { "head","body","arms","legs","tail","wings" });

        // Ferm?
        AddSupports(ar_vaca.Id, baseParts);
        AddSupports(ar_oaia.Id, baseParts);
        AddSupports(ar_calul.Id, new[] { "head","body","arms","legs","tail" });
        AddSupports(ar_gaina.Id, new[] { "head","body","arms","legs","tail","wings" });
        AddSupports(ar_porc.Id, baseParts);

        modelBuilder.Entity<AnimalPartSupport>().HasData(supports);

        // Config
        modelBuilder.Entity<BuilderConfig>().HasData(new BuilderConfig { Id = 1, BaseUnlockedAnimalCount = 3 });

        base.OnModelCreating(modelBuilder);
    }
}
