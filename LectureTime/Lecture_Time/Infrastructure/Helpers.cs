using Lecture_Time.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Web;

namespace Lecture_Time
{
    public static class Helpers
    {
        public static bool ValidateLDAP(LoginViewModel model)
        {
            string[] servers = { @"dc1.labs.wmi.amu.edu.pl", @"dc2.labs.wmi.amu.edu.pl" };
            string suffix = @"labs.wmi.amu.edu.pl";
            int port = 636;
            string root = @"DC=labs,DC=wmi,DC=amu,DC=edu,DC=pl";


            try
            {
                LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier(servers[0], port, true, false);
                LdapConnection lc = new LdapConnection(ldi);

                lc.AuthType = AuthType.Kerberos;

                String ldapUser = String.Format("{0}@{1}", model.UserName, suffix);
                lc.Credential = new NetworkCredential(ldapUser, model.Password);

                lc.Bind();
                return true;
            }
            catch (LdapException e)
            {
                if (e.Message.Contains("Podane poświadczenie jest nieprawidłowe.") || e.Message.Contains("The supplied credential is invalid."))
                {
                    return false;
                }
                throw e;
            }
            return false;

        }

        public static string[] GetUserData(String Login)
        {
            string[] servers = { @"dc1.labs.wmi.amu.edu.pl", @"dc2.labs.wmi.amu.edu.pl" };
            string suffix = @"labs.wmi.amu.edu.pl";
            int port = 636;
            string root = @"DC=labs,DC=wmi,DC=amu,DC=edu,DC=pl";

            string[] userData = new string[3];
            try
            {
                LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier(servers[0], port, true, false);
                LdapConnection lc = new LdapConnection(ldi);
                lc.AuthType = AuthType.Anonymous;
                lc.Bind();

                string filter = String.Format("(&(objectCategory=person)(sAMAccountName={0}))", Login);
                string[] attributesToReturn = { "sAMAccountName", "givenname", "sn", "mail" };

                SearchRequest sreq = new SearchRequest(root, filter, SearchScope.Subtree, attributesToReturn);
                SearchResponse sres = lc.SendRequest(sreq) as SearchResponse;

                string fname = (string)sres.Entries[0].Attributes["givenname"].GetValues(typeof(String))[0];
                string sname = (string)sres.Entries[0].Attributes["sn"].GetValues(typeof(String))[0];
                string email = (string)sres.Entries[0].Attributes["mail"].GetValues(typeof(String))[0];

                return new string[] { fname, sname, email };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static List<DateTime> GetCyclicDates(DateTime startTime, DateTime endTime, DayOfWeek dayOfWeek)
        {
            int interval = 20;//minutes

            var now = DateTime.Now.AddDays(1);
            var startDate = new DateTime(now.Year, now.Month, now.Day, startTime.Hour, startTime.Minute, 0);
            var endDate = new DateTime(now.Year, now.Month, now.Day, endTime.Hour, endTime.Minute, 0);
            List<DateTime> cyclicAppointmentDates = new List<DateTime>();
            var currDate = new DateTime(startDate.Ticks);

            for (int i = 0; i < 8; i++)
            {
                while (currDate<endDate)
                {
                    cyclicAppointmentDates.Add(Helpers.GetNextWeekday(currDate, dayOfWeek));
                    currDate = currDate.AddMinutes(interval);
                }
                startDate = startDate.AddDays(7);
                currDate = startDate;
                endDate = endDate.AddDays(7);
            }
            return cyclicAppointmentDates;
        }
    }
}