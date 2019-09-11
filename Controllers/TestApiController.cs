using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnetcore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestApiController : ControllerBase
    {
        // GET: api/TestApi
        [HttpGet]
        public IEnumerable<string> Get(int count)
        {
            long nthPrime = FindPrimeNumber(1000); //set higher value for more time

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < count; i++)
            {
                tasks.Add(Task.Run(() =>
                    {
                        slow();
                    }));
            }

            Task.WaitAll(tasks.ToArray());            

            return new string[] { nthPrime.ToString() };
        }

        private void slow()
        {
            var end = DateTime.Now + TimeSpan.FromSeconds(10);
            while (DateTime.Now < end)
                /*nothing here */
                ;
        }

        public long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;// to check if found a prime
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }

        //// GET: api/TestApi/5
        //[HttpGet("{id}", Name = "Get")]
        //[ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        //public Product Get(int id)
        //{
        //    return new Product() { ProductId=1, ProductName="Spoon" };
        //}

        // POST: api/TestApi
        [HttpPost]
        public ActionResult Post([FromBody] Product product)
        {
            if (ModelState.IsValid)
            {
                return new OkResult();
            }
            return new BadRequestResult();
        }

        // PUT: api/TestApi/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
