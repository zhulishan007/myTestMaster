using MARS_Revamp_DB.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;


namespace MarsApi.Controllers
{
    public class T_TESTER_INFOController : ApiController
    {
        private Entities db = new Entities();

        // GET: api/T_TESTER_INFO
        public IQueryable<T_TESTER_INFO> GetT_TESTER_INFO()
        {
            return db.T_TESTER_INFO;
        }

        // GET: api/T_TESTER_INFO/5
        [ResponseType(typeof(T_TESTER_INFO))]
        public async Task<IHttpActionResult> GetT_TESTER_INFO(decimal id)
        {
            T_TESTER_INFO t_TESTER_INFO = db.T_TESTER_INFO.Find(id);
            if (t_TESTER_INFO == null)
            {
                return NotFound();
            }

            return Ok(t_TESTER_INFO);
        }

        // PUT: api/T_TESTER_INFO/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutT_TESTER_INFO(decimal id, T_TESTER_INFO t_TESTER_INFO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != t_TESTER_INFO.TESTER_ID)
            {
                return BadRequest();
            }

            db.Entry(t_TESTER_INFO).State = System.Data.Entity.EntityState.Modified;

            try
            {
                 db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!T_TESTER_INFOExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/T_TESTER_INFO
        [ResponseType(typeof(T_TESTER_INFO))]
        public async Task<IHttpActionResult> PostT_TESTER_INFO(T_TESTER_INFO t_TESTER_INFO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.T_TESTER_INFO.Add(t_TESTER_INFO);

            try
            {
                 db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (T_TESTER_INFOExists(t_TESTER_INFO.TESTER_ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = t_TESTER_INFO.TESTER_ID }, t_TESTER_INFO);
        }

        // DELETE: api/T_TESTER_INFO/5
        [ResponseType(typeof(T_TESTER_INFO))]
        public async Task<IHttpActionResult> DeleteT_TESTER_INFO(decimal id)
        {
            T_TESTER_INFO t_TESTER_INFO =  db.T_TESTER_INFO.Find(id);
            if (t_TESTER_INFO == null)
            {
                return NotFound();
            }

            db.T_TESTER_INFO.Remove(t_TESTER_INFO);
             db.SaveChanges();

            return Ok(t_TESTER_INFO);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool T_TESTER_INFOExists(decimal id)
        {
            return db.T_TESTER_INFO.Count(e => e.TESTER_ID == id) > 0;
        }
    }
}