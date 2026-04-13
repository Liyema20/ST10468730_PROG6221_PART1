using System;

class Program
{
    static void Main(string[] args)
    {
        // Voice Greeting
        // Set a quicker speech rate so the bot reads content faster
        VoiceGreeting.SetSpeed(1);
        VoiceGreeting.PlayGreeting();

        // ASCII Logo
        AsciiArt.DisplayLogo();

        // Load users and check remembered user
        UserManager.LoadUsers();

        string name = null;

        // local exit helper used from menu or chat loop
        void ExitAndGoodbye(string userName)
        {
            var who = string.IsNullOrWhiteSpace(userName) ? string.Empty : $" {userName}";
            var message = $"Goodbye{who}. Stay safe online and make sure to be alert of any scams.";
            Console.WriteLine("\nBot: " + message);
            VoiceGreeting.Speak(message);
            Environment.Exit(0);
        }

        while (string.IsNullOrWhiteSpace(name))
        {
            var remembered = UserManager.GetRememberedUser();
            if (!string.IsNullOrEmpty(remembered))
            {
                Console.Write($"Auto-login as '{remembered}'? (Y/n): ");
                VoiceGreeting.Speak($"Auto login as {remembered}? Press Y to accept or N to decline.");
                var choice = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(choice) || choice.Trim().ToLower() == "y")
                {
                    name = remembered;
                    break;
                }
            }

            Console.WriteLine("1) Login");
            Console.WriteLine("2) Register");
            Console.WriteLine("3) Continue as Guest");
            Console.WriteLine("4) Settings");
            Console.WriteLine("5) Quit");
            Console.Write("Select an option: ");
            VoiceGreeting.Speak("Please select an option. One. Login. Two. Register. Three. Continue as guest. Four. Settings. Five. Quit.");
            var option = Console.ReadLine();

            if (option == "1")
            {
                Console.Write("Username: ");
                VoiceGreeting.Speak("Enter your username.");
                var u = Console.ReadLine();
                Console.Write("Password: ");
                VoiceGreeting.Speak("Enter your password.");
                var p = Console.ReadLine();
                Console.Write("Remember me? (y/N): ");
                VoiceGreeting.Speak("Remember me? Say yes to remember.");
                var r = Console.ReadLine();
                var remember = !string.IsNullOrWhiteSpace(r) && r.Trim().ToLower() == "y";

                if (UserManager.Login(u, p, remember))
                {
                    name = u;
                    Console.WriteLine("Login successful.");
                    VoiceGreeting.Speak("Login successful.");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid credentials. Try again.");
                    VoiceGreeting.Speak("Invalid credentials. Try again.");
                }
            }
            else if (option == "2")
            {
                Console.Write("Choose a username: ");
                VoiceGreeting.Speak("Choose a username.");
                var u = Console.ReadLine();
                Console.Write("Choose a password: ");
                VoiceGreeting.Speak("Choose a password.");
                var p = Console.ReadLine();
                Console.Write("Remember me? (y/N): ");
                VoiceGreeting.Speak("Remember me? Say yes to remember.");
                var r = Console.ReadLine();
                var remember = !string.IsNullOrWhiteSpace(r) && r.Trim().ToLower() == "y";

                if (UserManager.Register(u, p, remember))
                {
                    name = u;
                    Console.WriteLine("Registration successful. Logged in.");
                    VoiceGreeting.Speak("Registration successful. Logged in.");
                    break;
                }
                else
                {
                    Console.WriteLine("Registration failed. Username may already exist.");
                    VoiceGreeting.Speak("Registration failed. Username may already exist.");
                }
            }
            else if (option == "3")
            {
                Console.Write("Enter your display name: ");
                VoiceGreeting.Speak("Enter your display name.");
                name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name)) name = "Guest";
                VoiceGreeting.Speak($"Continuing as {name}.");
                break;
            }
            else if (option == "4")
            {
                OpenSettings();
            }
            else if (option == "5")
            {
                ExitAndGoodbye(null);
                return;
            }
            else
            {
                Console.WriteLine("Invalid option. Try again.");
                VoiceGreeting.Speak("Invalid option. Try again.");
            }
        }

        Chatbot bot = new Chatbot(name);

        // helper to write and speak bot messages
        static void BotSpeak(string text)
        {
            var full = "Bot: " + text;
            Console.WriteLine(full);
            VoiceGreeting.Speak(text);
        }

        // local settings function
        void OpenSettings()
        {
            Console.WriteLine("Settings - Voice");
            VoiceGreeting.Speak("Settings. Voice options.");
            Console.WriteLine("1) Female (friendly)");
            Console.WriteLine("2) Male");
            Console.WriteLine("3) Default system voice");
            Console.WriteLine("4) Custom voice name");
            Console.WriteLine("5) Disable voice");
            Console.Write("Choose voice preset: ");
            VoiceGreeting.Speak("Choose a voice preset. One for female. Two for male. Three for default. Four for custom. Five to disable voice.");
            var v = Console.ReadLine();
            if (v == "1")
            {
                VoiceGreeting.SetVoicePreset("female");
                VoiceGreeting.Speak("Voice set to female.");
            }
            else if (v == "2")
            {
                VoiceGreeting.SetVoicePreset("male");
                VoiceGreeting.Speak("Voice set to male.");
            }
            else if (v == "3")
            {
                VoiceGreeting.SetVoicePreset("default");
                VoiceGreeting.Speak("Voice set to default.");
            }
            else if (v == "4")
            {
                Console.Write("Enter custom voice name (platform-specific): ");
                VoiceGreeting.Speak("Enter custom voice name now.");
                var nameVoice = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nameVoice))
                {
                    VoiceGreeting.SetCustomVoice(nameVoice.Trim());
                    VoiceGreeting.Speak($"Custom voice set to {nameVoice}.");
                }
            }
            else if (v == "5")
            {
                VoiceGreeting.SetEnabled(false);
                Console.WriteLine("Voice disabled.");
            }

            Console.WriteLine("Test voice now? (y/N): ");
            var t = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(t) && t.Trim().ToLower().StartsWith("y"))
            {
                VoiceGreeting.Speak("This is a test of the selected voice. If it sounds good, you're all set.");
            }

            Console.WriteLine("Returning to menu...");
            VoiceGreeting.Speak("Returning to menu.");
        }

        Console.WriteLine();
        BotSpeak("Topics available:");
        var topics = ResponseManager.GetTopics();
        for (int i = 0; i < topics.Length; i++)
        {
            var line = $"{i + 1}. {topics[i]}";
            Console.WriteLine(line);
            VoiceGreeting.Speak(line);
        }

        BotSpeak("Type the topic number to learn more, type a question, or use commands: logout or exit. You can also type settings to change voice.");

        while (true)
        {
            Console.Write("\nYou: ");
            VoiceGreeting.Speak("Your input please.");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Bot: Please enter a valid question or command.");
                VoiceGreeting.Speak("Please enter a valid question or command.");
                continue;
            }

            var lowered = input.Trim().ToLower();
            if (lowered == "exit")
            {
                ExitAndGoodbye(name);
                break;
            }

            if (lowered == "logout")
            {
                UserManager.Logout(name);
                BotSpeak("Logged out.");
                // restart login flow
                Main(args);
                return;
            }

            if (lowered == "settings")
            {
                OpenSettings();
                // re-display topics
                Console.WriteLine();
                BotSpeak("Topics available:");
                for (int i = 0; i < topics.Length; i++)
                {
                    var line = $"{i + 1}. {topics[i]}";
                    Console.WriteLine(line);
                    VoiceGreeting.Speak(line);
                }
                continue;
            }

            // If input is a number corresponding to a topic
            if (int.TryParse(input.Trim(), out int idx))
            {
                if (idx >= 1 && idx <= ResponseManager.GetTopics().Length)
                {
                    // Interrupt any current speech so the new topic is spoken immediately
                    VoiceGreeting.Interrupt();
                    var parts = ResponseManager.GetTopicParts(idx - 1);

                    // Definition
                    BotSpeak("Definition: " + parts.Definition);

                    // Scenario
                    BotSpeak(parts.Scenario);

                    // Ask for confirmation
                    Console.Write("Do you fully understand this topic? (y/n): ");
                    VoiceGreeting.Speak("Do you fully understand this topic? Please answer yes or no.");
                    var confirm = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(confirm) && confirm.Trim().ToLower().StartsWith("y"))
                    {
                        // Provide solution
                        BotSpeak("Solution: " + parts.Solution);
                    }
                    else
                    {
                        // Ask if user wants a further layman's explanation
                        Console.Write("Would you like me to further explain this topic in plain terms? (y/n): ");
                        VoiceGreeting.Speak("Would you like me to further explain this topic in plain terms? Please answer yes or no.");
                        var explain = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(explain) && explain.Trim().ToLower().StartsWith("y"))
                        {
                            var lay = ResponseManager.GetLaymanExplanation(idx - 1);
                            foreach (var line in lay)
                            {
                                BotSpeak(line);
                            }

                            Console.Write("Do you fully understand this topic now? (y/n): ");
                            VoiceGreeting.Speak("Do you fully understand this topic now?");
                            var conf2 = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(conf2) && conf2.Trim().ToLower().StartsWith("y"))
                            {
                                BotSpeak("Solution: " + parts.Solution);
                            }
                            else
                            {
                                Console.WriteLine("Bot: Would you like to hear the solution or skip? (solution/skip)");
                                VoiceGreeting.Speak("Would you like to hear the solution or skip?");
                                var c2 = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(c2) && c2.Trim().ToLower().StartsWith("sol"))
                                {
                                    BotSpeak("Solution: " + parts.Solution);
                                }
                                else
                                {
                                    BotSpeak("Okay, returning to main menu.");
                                }
                            }
                        }
                        else
                        {
                            // If user declines further explanation, offer solution or skip
                            Console.WriteLine("Bot: Would you like to hear the solution or skip? (solution/skip)");
                            VoiceGreeting.Speak("Would you like to hear the solution or skip?");
                            var choice = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(choice) && choice.Trim().ToLower().StartsWith("sol"))
                            {
                                BotSpeak("Solution: " + parts.Solution);
                            }
                            else
                            {
                                BotSpeak("Okay, returning to main menu.");
                            }
                        }
                    }

                    // After completing topic flow, offer to go back to menu
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to return to the main menu...");
                    VoiceGreeting.Speak("Press Enter to return to the main menu.");
                    Console.ReadLine();
                    continue;
                }
            }

            if (!InputValidator.IsValid(input))
            {
                Console.WriteLine("Bot: Please enter a valid question.");
                VoiceGreeting.Speak("Please enter a valid question.");
                continue;
            }

            string response = bot.GetResponse(input);
            BotSpeak(response);

            // After bot response, allow user to return to menu
            Console.WriteLine();
            Console.WriteLine("Press Enter to return to the main menu...");
            VoiceGreeting.Speak("Press Enter to return to the main menu.");
            Console.ReadLine();
        }
    }
}