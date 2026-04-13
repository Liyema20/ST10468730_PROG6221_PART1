using System;

public static class ResponseManager
{
    public static string GenerateResponse(string input)
    {
        input = input.ToLower();

        // common ways users ask how the bot is
        if (input.Contains("how are you") || input.Contains("how r you") || input.Contains("how are u") ||
            input.Contains("how's it going") || input.Contains("how is it going") || input.Contains("how ya doing") ||
            input.Trim() == "how" || input.Trim() == "sup")
        {
            return "I'm good! Ready to help you stay safe online 💻";
        }

        // common ways users ask the bot's purpose or what it does
        if (input.Contains("purpose") || input.Contains("what do you do") || input.Contains("what is your purpose") ||
            input.Contains("what's your purpose") || input.Contains("who are you") || input.Contains("tell me about yourself"))
        {
            return "I help teach cybersecurity awareness: I explain risks like phishing, password hygiene, safe browsing, two-factor authentication, and social engineering — with examples and simple solutions.";
        }

        if (input.Contains("phishing"))
            return "Phishing is when scammers trick you into giving personal info through fake emails or links.";

        if (input.Contains("password"))
            return "Use strong passwords with letters, numbers, and symbols. Never share them!";

        if (input.Contains("safe browsing"))
            return "Always check URLs, avoid suspicious links, and use secure websites (https).";

        return "I didn't understand that. Try asking about cybersecurity topics.";
    }

    public static string[] GetTopics()
    {
        return new[]
        {
            "Phishing",
            "Password Hygiene",
            "Safe Browsing",
            "Two-Factor Authentication",
            "Social Engineering"
        };
    }
    public static (string Definition, string Scenario, string Solution) GetTopicParts(int index)
    {
        switch (index)
        {
            case 0:
                return (
                    "Phishing",
                    "Scenario: You receive an email appearing to be from your bank asking you to click a link and verify your password to avoid account suspension.",
                    "Solution: Do not click the link. Visit the bank's website directly or call their support. Verify sender address and enable email security features."
                );
            case 1:
                return (
                    "Password Hygiene",
                    "Scenario: You reuse the same simple password across multiple sites. One breach exposes your accounts everywhere.",
                    "Solution: Use a password manager, create unique strong passwords for each site, and enable multi-factor authentication."
                );
            case 2:
                return (
                    "Safe Browsing",
                    "Scenario: You download an attachment from an unknown site that claims to offer a free tool, which installs malware.",
                    "Solution: Only download from trusted sources, check URLs for typos, enable browser security features, and keep software updated."
                );
            case 3:
                return (
                    "Two-Factor Authentication",
                    "Scenario: An attacker obtains your password but cannot access your account because a second factor (authenticator app) is required.",
                    "Solution: Enable 2FA using authenticator apps or hardware keys; avoid SMS when possible due to SIM-swapping risks."
                );
            case 4:
                return (
                    "Social Engineering",
                    "Scenario: Someone calls claiming to be IT support and asks for your credentials to fix an issue.",
                    "Solution: Verify identity through known channels, do not share credentials, and report suspicious requests to your organization."
                );
            default:
                return ("Unknown", "No scenario available.", "No solution available.");
        }
    }

    // Returns a layman's explanation (multiple short lines) for the topic at index.
    public static string[] GetLaymanExplanation(int index)
    {
        switch (index)
        {
            case 0: // Phishing
                return new[]
                {
                    "Phishing is like a trick where someone pretends to be someone you trust.",
                    "They might send an email or message that looks real but is fake.",
                    "The message often asks you to click a link or give personal information.",
                    "If you click the link, you may be taken to a fake website that steals your login.",
                    "Attackers use small details like logos or similar addresses to seem real.",
                    "Always check the sender address and don’t rush — scammers create urgency.",
                    "Never type your password on a site you reached from an email link.",
                    "When unsure, go directly to the real website by typing the address yourself.",
                    "Use two-factor authentication so stolen passwords alone won’t give access.",
                    "Report suspicious messages and delete them so others aren’t tricked."
                };
            case 1: // Password Hygiene
                return new[]
                {
                    "Using the same simple password everywhere is risky.",
                    "If one site is breached, attackers try that password on other sites.",
                    "A strong password is long, unique, and uses a mix of characters.",
                    "A password manager remembers complex passwords so you don't have to.",
                    "Think of a password manager as a locked safe for all your keys.",
                    "Change passwords after a known breach and avoid obvious phrases.",
                    "Enable two-factor authentication so a code or app is also required.",
                    "Avoid writing passwords down where others can find them easily.",
                    "Use long passphrases instead of single words when possible.",
                    "Regularly review and remove old accounts you no longer use."
                };
            case 2: // Safe Browsing
                return new[]
                {
                    "Safe browsing means being careful about websites and downloads.",
                    "Fake sites can look real but install malware or steal information.",
                    "Always check the web address for typos or odd characters.",
                    "Look for the padlock and https — but know that alone isn't perfect.",
                    "Only download software from official or well-known sources.",
                    "Avoid clicking random ads or popup buttons that promise free stuff.",
                    "Keep your browser and plugins up to date to patch security holes.",
                    "Use strong, unique passwords and avoid entering them on untrusted pages.",
                    "Consider using browser security extensions and antivirus software.",
                    "If something seems too good to be true, it probably is — stop and check."
                };
            case 3: // Two-Factor Authentication
                return new[]
                {
                    "Two-factor authentication (2FA) adds a second check beyond your password.",
                    "Even if someone knows your password, they still need the second factor.",
                    "Common second factors are codes from an app, SMS, or a hardware key.",
                    "Authenticator apps are safer than SMS because phones can be swapped.",
                    "Think of 2FA like a second lock on your door — harder for thieves to get in.",
                    "Turn on 2FA for email, banking, and important accounts whenever offered.",
                    "Back up recovery codes somewhere safe in case you lose access to the app.",
                    "Avoid reuse of backup codes and keep them offline if possible.",
                    "Hardware keys (USB) offer very strong protection if supported.",
                    "Using 2FA dramatically reduces the risk from stolen passwords."
                };
            case 4: // Social Engineering
                return new[]
                {
                    "Social engineering is when someone manipulates you instead of hacking tech.",
                    "They use psychology — trust, urgency, fear — to get you to act quickly.",
                    "Common examples are fake IT calls or scam messages asking for help.",
                    "Always verify identities through official channels before sharing info.",
                    "Don’t give passwords or codes to people who call or message unexpectedly.",
                    "Slow down and question requests that seem unusual or pushy.",
                    "Ask for a callback number and check it against official contact details.",
                    "Report suspicious contacts to your organization or service provider.",
                    "Train yourself to spot red flags like grammar errors or odd sender details.",
                    "Treat unexpected requests for sensitive actions as potential scams."
                };
            default:
                return new[] { "No layman's explanation available for this topic." };
        }
    }
}