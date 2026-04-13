using System;

public class Chatbot
{
    private string userName;

    public Chatbot(string name)
    {
        userName = name;
        var message = $"Welcome {userName}! Let's stay safe online 🔐";
        Console.WriteLine("\nBot: " + message);
        VoiceGreeting.Speak(message);
    }

    public string GetResponse(string input)
    {
        return ResponseManager.GenerateResponse(input);
    }
}