﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnetcore.Models;
using System.IO;

namespace aspnetcore.Controllers
{
    public class HomeController : Controller
    {
        NodeEnvironment nodeEnvironment = new NodeEnvironment();
        public IActionResult Index()
        {
            nodeEnvironment.MachineName = Environment.MachineName;
            return View(nodeEnvironment);
        }

        public void CreateInCurrentDirectory()
        {
            try
            {
                Console.WriteLine(Environment.CurrentDirectory);
               DirectoryInfo di = Directory.CreateDirectory(Environment.CurrentDirectory + "\\testdir");
                
            }
            catch(Exception ex)
            {
                Console.WriteLine("error occurred");
            }
        }

        public void CreateTestFile()
        {
            //Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\test");
            try
            {
                Console.WriteLine("Inside CreateTestFile");
                string mountPath = "/myfileshare";
                Directory.CreateDirectory(mountPath + "//test");
                using (StreamWriter sw = new StreamWriter(mountPath + "//123.txt"))
                {
                    sw.WriteLine("hello");
                }
                Console.WriteLine("File created successfully");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
