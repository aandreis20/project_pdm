using System.Text.Json;
using Microsoft.Maui.Storage;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace RecipesApp.Services
{
    public class SupabaseSessionHandler : IGotrueSessionPersistence<Session>
    {
        private const string SessionKey = "supabase_session";

        public void SaveSession(Session session)
        {
            var json = JsonSerializer.Serialize(session);
            SecureStorage.SetAsync(SessionKey, json).GetAwaiter().GetResult();
        }

        public void DestroySession()
        {
            SecureStorage.Remove(SessionKey);
        }

        public Session? LoadSession()
        {
            var json = SecureStorage.GetAsync(SessionKey).GetAwaiter().GetResult();
            if (json == null) return null;
            return JsonSerializer.Deserialize<Session>(json);
        }
    }
}