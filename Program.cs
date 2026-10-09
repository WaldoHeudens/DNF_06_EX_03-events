// Een event is een delegate met bewaking eromheen. Buitenstaanders mogen
// zich abonneren (+=) en afmelden (-=), maar enkel de klasse zelf mag het
// event afvuren. Zo bouw je losgekoppelde, event-driven code.

var teller = new Teller();

// Abonneren met een lambda. Deze method loopt telkens het event afvuurt.
teller.DrempelBereikt += (afzender, e) =>
    Console.WriteLine($"Drempel bereikt bij stand {e.Stand}!");

// Een tweede abonnee. Een event mag meerdere abonnees hebben (multicast).
teller.DrempelBereikt += LogNaarConsole;

// We tellen op tot voorbij de drempel. Het event vuurt vanzelf af.
for (int i = 0; i < 5; i++)
{
    teller.Tel();
}

static void LogNaarConsole(object? afzender, DrempelEventArgs e) =>
    Console.WriteLine($"[log] drempel geraakt op {e.TijdstipTekst}");

// De klasse die het event bezit en afvuurt.
class Teller
{
    private int stand;

    // De drempel is een expression-bodied property: een korte schrijfwijze
    // voor een property met enkel een get.
    public int Drempel => 3;

    // Het event. EventHandler<T> is de standaardvorm in .NET:
    //   (object? afzender, T eventargs) => void
    public event EventHandler<DrempelEventArgs>? DrempelBereikt;

    public void Tel()
    {
        stand++;
        Console.WriteLine($"stand = {stand}");

        if (stand == Drempel)
        {
            // Afvuren. De ?.Invoke doet niets als er geen abonnees zijn.
            DrempelBereikt?.Invoke(this, new DrempelEventArgs(stand));
        }
    }
}

// Eigen EventArgs: hierin stop je de gegevens die bij het event horen.
// Het tijdstip leggen we vast bij het aanmaken, dus op het moment dat het
// event afvuurt, niet pas wanneer een abonnee de property uitleest.
class DrempelEventArgs(int stand) : EventArgs
{
    public int Stand => stand;
    public DateTime Tijdstip { get; } = DateTime.Now;
    public string TijdstipTekst => Tijdstip.ToString("HH:mm:ss");
}
