using Google.Protobuf.WellKnownTypes;
using DomainAdres = Hackvip.Domain.Adres;
using DomainBurger = Hackvip.Domain.Burger;

namespace Hackvip.Models;

public record Adres(
  string Straatnaam,
  string Huisnummer,
  string Postcode,
  string Woonplaats
)
{
  public static Adres Empty => new("", "", "", "");
  public DomainAdres ToProtobufModel() => new()
  {
    Straatnaam = Straatnaam,
    Huisnummer = Huisnummer,
    Postcode = Postcode,
    Woonplaats = Woonplaats
  };
}
public record BurgerModel(
  string Bsn,
  string Voorletters,
  string Achternaam,
  Adres Adres
)
{
  public static BurgerModel Empty => new("", "", "", Adres.Empty);
  public DomainBurger ToProtobufModel() => new()
  {
    Bsn = Bsn,
    Voorletters = Voorletters,
    Achternaam = Achternaam,
    Adres = Adres.ToProtobufModel()
  };
}
