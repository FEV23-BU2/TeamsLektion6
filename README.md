---
author: Lektion 6
---

# Teams lektion 6

Hej och välkommen!

## Agenda

1. Svar på frågor
2. Repetition
3. Genomgång av övningar
4. Skolsystem
5. Övning och handledning

---

# Fråga

Varför behövs tomma constructors?

# Svar

Ibland så kan inte ASP.NET lista ut hur ett objekt kan skapas, och därför krävs en tom constructor så att värden sedan kan läggas in.

---

# Fråga

Hur hanteras data i verkligheten? Använd en webbshop som exempel. Hur hanteras produkter? Hårdkodas dem på frontenden eller ligger dem i databasen?

# Svar

Detta varierar från projekt till projekt. Data som sällan ändras kan hårdkodas från frontend utan större problem. Data som ändras någorlunda ofta bör sparas i en databas och bör också kopplas till en CMS.

---

# Fråga

Hur hanteras och kopplas email till en webbsida?

# Svar

<https://mimekit.net/>

```csharp
var message = new MimeMessage();
message.From.Add(new MailboxAddress(name: "John Doe", address: "joe@inbox.test"));
message.To.Add(new MailboxAddress(name: "Bruce Williams", address: "bruce@inbox.test"));
message.Subject = "Test subject";

message.Body = new TextPart("plain")
{
    Text = "Test body"
};

using (var client = new SmtpClient())
{
    client.Connect(host: "mail.inbox.example", port: 587, useSsl: true);
    client.Authenticate(userName: "test", password: "test");
    client.Send(message);
    client.Disconnect(quit: true);
}
```

---

# Fråga

Hur, exakt, bör saker delas upp i olika filer när det kommer till controllers, services och så vidare?

# Svar

Titta på skolsystemet som exempel.
