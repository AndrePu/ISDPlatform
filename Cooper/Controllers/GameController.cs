﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cooper.Models;
using Cooper.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Cooper.Configuration;
using Microsoft.AspNetCore.Http;

namespace Cooper.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GameController : ControllerBase
    {
        private readonly IRepository<Game> gameRepository;

        public GameController(IConfigProvider configProvider)
        {
            gameRepository = new GameRepository(configProvider);
        }

        /// <summary>
        /// Returns all games.
        /// </summary>
        /// <returns>All games</returns>
        /// <response code="200">Always</response>
        [HttpGet]
        [Route("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Game>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            return Ok(gameRepository.GetAll());
        }

        /// <summary>
        /// Returns game by id.
        /// </summary>
        /// <returns>Game</returns>
        /// <response code="200">If the game exists</response>
        /// <response code="404">If the game does not exist</response>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetGameById(long id)
        {
            Game game = gameRepository.Get(id);

            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }
        
        /// <summary>
        /// Creates game.
        /// </summary>
        /// <returns>Game</returns>
        /// <response code="200">If the game is created</response>
        /// <response code="400">If the model state is invalid</response>
        [HttpPost]
        [Route("{id}")]
        [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post(Game game, string Token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(Token);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else if (game.Id == 0)
            {
                long id = gameRepository.Create(game);
                game.Id = id;

                return Ok(game);
            }
            else
            {
                gameRepository.Update(game);

                return Ok(game);
            }
        }
        
        /// <summary>
        /// Deletes game.
        /// </summary>
        /// <returns>Status code 200</returns>
        /// <response code="200">Always</response>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Delete(long id)
        {
            gameRepository.Delete(id);
            return Ok();
        }
    }
}
