namespace XooCreator.BA.Features.Builder;

public static class BuilderData
{
    public static object GetCreatureBuilderData()
    {
        // This mirrors the data structure from specs.txt in a minimal, serializable form
        var parts = new[]
        {
            new { key = "head", name = "Head", image = "images/bodyparts/face.webp" },
            new { key = "body", name = "Body", image = "images/bodyparts/body.webp" },
            new { key = "arms", name = "Arms", image = "images/bodyparts/hands.webp" },
            new { key = "legs", name = "Legs", image = "images/bodyparts/legs.webp" },
            new { key = "tail", name = "Tail", image = "images/bodyparts/tail.webp" },
            new { key = "wings", name = "Wings", image = "images/bodyparts/wings.webp" },
            new { key = "horn", name = "Horn", image = "images/bodyparts/horn.webp" },
            new { key = "horns", name = "Horns", image = "images/bodyparts/horns.webp" },
        };

        string[] baseParts = new[] { "head", "body", "arms" };

        var animals = new[]
        {
            new { src = "images/animals/base/bunny.jpg",   label = "Bunny",   supports = baseParts },
            new { src = "images/animals/base/cat.jpg",     label = "Cat",     supports = baseParts },
            new { src = "images/animals/base/giraffe.jpg", label = "Giraffe", supports = new[] { "head","body","arms","legs","tail","wings","horn","horns" } },
            new { src = "images/animals/base/dog.jpg",     label = "Dog",     supports = baseParts },
            new { src = "images/animals/base/fox.jpg",     label = "Fox",     supports = baseParts },
            new { src = "images/animals/base/hippo.jpg",   label = "Hippo",   supports = baseParts },
            new { src = "images/animals/base/monkey.jpg",  label = "Monkey",  supports = baseParts },
            new { src = "images/animals/base/camel.jpg",   label = "Camel",   supports = baseParts },
            new { src = "images/animals/base/deer.jpg",    label = "Deer",    supports = new[] { "head","body","arms","legs","tail","wings","horn","horns" } },
            new { src = "images/animals/base/duck.jpg",    label = "Duck",    supports = new[] { "head","body","arms","legs","tail","wings" } },
            new { src = "images/animals/base/eagle.jpg",   label = "Eagle",   supports = new[] { "head","body","arms","legs","tail","wings" } },
            new { src = "images/animals/base/elephant.jpg",label = "Elephant",supports = baseParts },
            new { src = "images/animals/base/ostrich.jpg", label = "Ostrich", supports = new[] { "head","body","arms","legs","tail","wings" } },
            new { src = "images/animals/base/parrot.jpg",  label = "Parrot",  supports = new[] { "head","body","arms","legs","tail","wings" } },
        };

        var baseLockedParts = new[] { "legs", "tail", "wings", "horn", "horns" };

        return new
        {
            parts,
            animals,
            baseUnlockedAnimalCount = 3,
            baseLockedParts
        };
    }
}
