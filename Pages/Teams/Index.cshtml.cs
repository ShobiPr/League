using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using League.Models;
using League.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace League.Pages.Teams
{
    public class IndexModel : PageModel
    {
        // inject ET context
        private readonly LeagueContext _context;

        public IndexModel(LeagueContext context)
        {
            _context = context;
        }

        // allow for storage of favorite team
        [BindProperty(SupportsGet =true)]
        public string FavoriteTeam { get; set; }

        public SelectList AllTeams { get; set; }

        // load all leauges, divisions and teams
        public List<Conference> Conferences { get; set; }
        public List<Division> Divisions { get; set; }
        public List<Team> Teams { get; set; }

        public async Task OnGetAsync()
        {
            // load all records from three tables 
            Conferences = await _context.Conferences.ToListAsync();
            Divisions = await _context.Divisions.ToListAsync();
            Teams = await _context.Teams.ToListAsync();

            // make a list of teams for the favorite select dropdown
            IQueryable<string> teamQuery = from t in _context.Teams
                                           orderby t.TeamId
                                           select t.TeamId;

            AllTeams = new SelectList(await teamQuery.ToListAsync());

            if(FavoriteTeam != null)
            {
                HttpContext.Session.SetString("_Favorite", FavoriteTeam);
            }
            else
            {
                FavoriteTeam = HttpContext.Session.GetString("_Favorite");
            }

        }

        //get all divisions in a conference and sort them 
        public List<Division> GetConferenceDivisions(string ConferenceId)
        {
            return Divisions.Where(d => d.ConferenceId.Equals(ConferenceId)).OrderBy(d => d.Name).ToList();
        }

        // get all teams in a division ordered by win/loss record
        public List<Team> GetDivisionTeams(string DivisionID)
        {
            return Teams.Where(t => t.DivisionId.Equals(DivisionID)).OrderByDescending(t => t.Win).ToList();
        }
    }
}
