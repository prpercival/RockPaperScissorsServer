using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RockPaperScissorsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        public static List<Lobby> Lobbies = new List<Lobby>();

        // GET: api/<controller>
        [HttpGet, Route("getlobbies")]
        public IEnumerable<Lobby> Get()
        {
            if (Lobbies.Count == 0)
                return null;

            //var temp = new List<Lobby>(Lobbies);

            for(int i = 0; i < Lobbies.Count(); i++)
            {
                var lobby = Lobbies.ElementAt(i);

                for (int j = 0; j < lobby.Users.Count(); j++)
                {
                    var user = lobby.Users.ElementAt(j);

                    if (user.Value.Expiration < DateTime.Now)
                    {
                        lobby.Users.Remove(user.Key);
                    }
                }

                if(lobby.Users.Count == 0)
                {
                    Lobbies.Remove(lobby);
                }
            }

            return Lobbies;
        }

        // GET: api/<controller>
        [HttpPost, Route("joinlobby")]
        public Lobby JoinLobby([FromBody]Lobby lobby, string userId)
        {
            var serverLobby = Lobbies.Where(x => x.Id == lobby.Id).FirstOrDefault();

            if (serverLobby.Users.Count >= 2)
                return null;

            lobby.Users.TryGetValue(userId, out var user);           

            serverLobby.Users.Add(userId, user);

            return serverLobby;
        }

        [HttpPost, Route("getlobby")]
        public Lobby GetLobby([FromBody]Lobby lobby)
        {
            var serverLobby = Lobbies.Where(x => x.Id == lobby.Id).FirstOrDefault();

            return serverLobby;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost, Route("addlobby")]
        public IEnumerable<Lobby> AddLobby([FromBody]User user)
        {
            var users = new Dictionary<string, User>() { { user.Id, user } };
            Lobbies.Add(new Lobby() { Name = user.Name, Size = 1, Users = users, Id = Guid.NewGuid() });

            return Lobbies;
        }

        // POST api/<controller>
        [HttpPost, Route("heartbeat")]
        public Lobby HeartBeat(string userId)
        {
            var serverLobby = Lobbies.Where(x => x.Users.ContainsKey(userId)).FirstOrDefault();

            serverLobby.Users.TryGetValue(userId, out var user);

            user.Expiration = DateTime.Now.AddSeconds(5);

            return serverLobby;
        }

        // PUT api/<controller>/5
        [HttpPost, Route("shoot")]
        public Lobby Shoot([FromBody]User user)
        {
            var serverLobby = Lobbies.Where(x => x.Users.ContainsKey(user.Id)).FirstOrDefault();

            if (serverLobby == null)
                return null;

            serverLobby.Users.TryGetValue(user.Id, out var serverUser);

            serverUser.Choice = user.Choice;

            return serverLobby;
        }

        // POST api/<controller>
        [HttpPost, Route("leavelobby")]
        public Lobby Reset([FromBody]User user, string lobbyId)
        {
            var serverLobby = Lobbies.Where(x => x.Id == new Guid(lobbyId)).FirstOrDefault();

            serverLobby.Users.Remove(user.Id);

            if (serverLobby.Users.Count == 0)
                Lobbies.Remove(serverLobby);

            return serverLobby;
        }

        // POST api/<controller>
        [HttpPost, Route("reset")]
        public Lobby Reset([FromBody]Lobby lobby)
        {
            var serverLobby = Lobbies.Where(x => x.Id == lobby.Id).FirstOrDefault();

            foreach(var user in serverLobby.Users)
            {
                user.Value.Choice = null;
            }

            return serverLobby;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
