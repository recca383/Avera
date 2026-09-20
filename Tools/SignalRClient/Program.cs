using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Headers;
using System.Net.Http.Json;


await Task.WhenAll(Perform());

static async Task Perform()
{
    Console.WriteLine("SignalR Test Client");

    Console.Write("API Base URL (e.g. https://localhost:7280): ");
    var baseUrl = "https://localhost:7280";

    Console.Write("Admin Bearer token (leave blank to skip admin client): ");
    var adminToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJBdmVyYS1BdWRpZW5jZSIsImlzcyI6InJlY2NhMzgzIiwiZXhwIjoxNzg5OTg5OTMyLCJpYXQiOjE3ODk5MDM1MzIsIm5iZiI6MTc4OTkwMzUzMiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIwMWEwYmU2ZC0zNTVhLTc0ODctYjYwYi01NjA5YTg0N2M3NWEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJjcnVzaXR3aW5jZWxAZ21haWwuY29tIiwiU2VjdXJpdHlTdGFtcCI6IkVUUFlCNFNNQ1pBSUNCWUtSRE9IN0Q2VkhNQzU1RVM2IiwiVGVuYW50SWQiOiIyODBhZmVjZS0wOWNjLTQyMDItODJhYS0zYTQ1ZGZmN2FlYTEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiJ9.E2PhA91smkbEqocXM8tRcdf7_oyXosDDhFMbhU3UcY4";

    Console.Write("User Bearer token (leave blank to skip user client): ");
    var userToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJBdmVyYS1BdWRpZW5jZSIsImlzcyI6InJlY2NhMzgzIiwiZXhwIjoxNzg5OTg5OTU0LCJpYXQiOjE3ODk5MDM1NTQsIm5iZiI6MTc4OTkwMzU1NCwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIwMWEwYmU3Ny01NDYwLTdhZTEtYmFhZi03MTliM2QyN2JlYzQiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ3d3cuby5mLmYuaS5jLmkuYS5sMEBnbWFpbC5jb20iLCJTZWN1cml0eVN0YW1wIjoiVVg3U1JORE0yTkJONTVJQ1VMQ0NZVEE1REVNT1lEU1UiLCJUZW5hbnRJZCI6ImJlMzhkOGE4LTljZWEtNGUyMy1iYTQyLWRlZjQ0MmNmZjU0YyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIifQ.PixQvXY0RHoFVG0fOWaFIPj6MBv5TY-wZgT6uPdvCw8";

    var adminConnection = (!string.IsNullOrEmpty(adminToken)) ? CreateConnection(baseUrl, adminToken, "Admin") : null;
    var userConnection = (!string.IsNullOrEmpty(userToken)) ? CreateConnection(baseUrl, userToken, "User") : null;

    if (adminConnection != null)
        await adminConnection.StartAsync();
    if (userConnection != null)
        await userConnection.StartAsync();

    Console.WriteLine("Connections started. Listening for events...");
    Console.WriteLine("Commands: join, approve, reject, suspend, remove, analyze, flag, exit");

    var http = new HttpClient() { BaseAddress = new Uri(baseUrl) };
    // By default use admin token for commands that require admin. If you want to act as user, run separate instance with user token as adminToken.
    if (!string.IsNullOrEmpty(adminToken)) http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

    while (true)
    {
        Console.Write("> ");
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) continue;
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cmd = parts[0].ToLowerInvariant();

        try
        {
            if (cmd == "exit") break;

            if (cmd == "join")
            {
                Console.Write("Invite code: ");
                var invite = Console.ReadLine();
                var payload = new { InviteCode = invite };
                var resp = await http.PostAsJsonAsync("/auth/join-invite-code", payload);
                Console.WriteLine($"Join invite response: {resp.StatusCode}");
                continue;
            }

            if (cmd == "approve")
            {
                Console.Write("MemberRequestId: ");
                var id = Guid.Parse(Console.ReadLine()!);
                var resp = await http.PostAsync($"tenants/member-requests/{id}/approve", null);
                Console.WriteLine($"Approve response: {resp.StatusCode}");
                continue;
            }

            if (cmd == "reject")
            {
                Console.Write("MemberRequestId: ");
                var id = Guid.Parse(Console.ReadLine()!);
                var resp = await http.PostAsync($"tenants/member-requests/{id}/reject", null);
                Console.WriteLine($"Reject response: {resp.StatusCode}");
                continue;
            }

            if (cmd == "suspend")
            {
                Console.Write("UserId: ");
                var id = Guid.Parse(Console.ReadLine()!);
                var resp = await http.PostAsJsonAsync("/admin/suspend-user", id);
                Console.WriteLine($"Suspend response: {resp.StatusCode}");
                continue;
            }

            if (cmd == "remove")
            {
                Console.Write("UserId: ");
                var id = Guid.Parse(Console.ReadLine()!);
                var resp = await http.DeleteAsync($"/tenant/members/{id}");
                Console.WriteLine($"Remove response: {resp.StatusCode}");
                continue;
            }

            if (cmd == "analyze")
            {
                Console.Write("CaseId: ");
                var id = Guid.Parse(Console.ReadLine()!);
                var resp = await http.GetAsync($"cases/{id}/analysis");
                Console.WriteLine($"Analyze response: {resp.StatusCode}");
                continue;
            }

            if (cmd == "flag")
            {
                Console.Write("CaseId: ");
                var id = Guid.Parse(Console.ReadLine()!);
                Console.Write("IsFlagged (true/false): ");
                var flag = bool.Parse(Console.ReadLine()!);
                var payload = new { IsFlagged = flag };
                var resp = await http.PostAsJsonAsync($"cases/{id}/flag", payload);
                Console.WriteLine($"Flag response: {resp.StatusCode}");
                continue;
            }

            Console.WriteLine("Unknown command");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    if (adminConnection != null) await adminConnection.StopAsync();
    if (userConnection != null) await userConnection.StopAsync();

    static HubConnection CreateConnection(string baseUrl, string token, string label)
    {
        var hub = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}/hubs/notification", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect()
            .Build();

        hub.Closed += async (ex) => { Console.WriteLine($"{label} disconnected: {ex?.Message}"); await Task.Delay(1000); };

        // Common handlers
        hub.On<object>("MemberRequestCreated", (payload) => Console.WriteLine($"[{label}] MemberRequestCreated: {System.Text.Json.JsonSerializer.Serialize(payload)}"));
        hub.On<object>("MemberRequestApproved", (payload) => Console.WriteLine($"[{label}] MemberRequestApproved: {System.Text.Json.JsonSerializer.Serialize(payload)}"));
        hub.On<object>("MemberRequestRejected", (payload) => Console.WriteLine($"[{label}] MemberRequestRejected: {System.Text.Json.JsonSerializer.Serialize(payload)}"));
        hub.On<object>("NewCaseResult", (payload) => Console.WriteLine($"[{label}] NewCaseResult: {System.Text.Json.JsonSerializer.Serialize(payload)}"));
        hub.On<object>("CaseReviewCompleted", (payload) => Console.WriteLine($"[{label}] CaseReviewCompleted: {System.Text.Json.JsonSerializer.Serialize(payload)}"));
        hub.On<object>("UserSuspended", (payload) => Console.WriteLine($"[{label}] UserSuspended: {System.Text.Json.JsonSerializer.Serialize(payload)}"));
        hub.On<object>("UserRemoved", (payload) => Console.WriteLine($"[{label}] UserRemoved: {System.Text.Json.JsonSerializer.Serialize(payload)}"));
        hub.On<object>("UserDeleted", (payload) => Console.WriteLine($"[{label}] UserDeleted: {System.Text.Json.JsonSerializer.Serialize(payload)}"));
        hub.On<object>("CaseFlagged", (payload) => Console.WriteLine($"[{label}] CaseFlagged: {System.Text.Json.JsonSerializer.Serialize(payload)}"));

        return hub;
    }
}
