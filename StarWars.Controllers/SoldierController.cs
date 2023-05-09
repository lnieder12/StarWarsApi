﻿using Microsoft.AspNetCore.Mvc;
using StarWars.Model;

namespace StarWars.Controllers;

[Route("[controller]")]
[ApiController]
public class SoldierController : GenericController<Soldier>
{
    public SoldierController(StarWarsDbContext context) : base(context)
    {
    }
}
