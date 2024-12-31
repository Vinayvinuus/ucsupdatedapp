using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ucsUpdatedApp.Data;
using ucsUpdatedApp.Models;
using System.Linq;

namespace ucsUpdatedApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        public class AttendanceRequest
        {
            public int? EmployeeId { get; set; }
            public string FingerPrintData { get; set; }
            public int Op { get; set; } // 1 for Check-In, 0 for Check-Out
            public DateTime OpDateTime { get; set; }
        }

        public class AttendanceResponse
        {
            public int Id { get; set; }
            public int EmployeeId { get; set; }
            public string Operation { get; set; }
            public DateTime OpDateTime { get; set; }
            // public string CheckInMethod { get; set; }
            public string Status { get; set; }
        }
        [HttpGet("master")]
        public async Task<IActionResult> GetMasterData()
        {
            try
            {
                var masterData = await _context.MasterTable.OrderBy(m => m.MasterId)
                    .Select(m => new
                    {
                        m.MasterId,
                        m.EmployeeId,
                        m.Employeename,
                        m.FingerPrintData,
                        DOB = m.DOB.HasValue ? m.DOB.Value.ToString("yyyy-MM-dd") : null, //yyyy-MM-dd
                        DOJ = m.DOJ.HasValue ? m.DOJ.Value.ToString("yyyy-MM-dd") : null

                        // m.LastTransactionDate
                    })
                    .ToListAsync();

                return Ok(masterData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactionData()
        {
            try
            {
                var transactionData = await _context.TransactionTable
                    .OrderByDescending(t => t.OpDateTime)
                    .Select(t => new
                    {
                        //t.Id,
                        t.MasterId,
                        t.Op,
                        OperationType = t.Op == 1 ? "Check-In" : "Check-Out",
                        t.OpDateTime,
                        // t.CheckInMethod,

                        //  t.Master.MasterId,
                        t.Master.Employeename,
                        t.Master.LastTransactionDate


                    })
                    .ToListAsync();

                return Ok(transactionData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost]

                public async Task<IActionResult> ManageAttendance([FromBody] List<AttendanceRequest> requests)
        {
            if (requests == null || requests.Count == 0)
            {
                return BadRequest("Request list cannot be empty.");
            }

            var responses = new List<AttendanceResponse>();

            foreach (var request in requests)
            {
                try
                {
                    // Determine check-in method
                    string checkInMethod = request.EmployeeId.HasValue && !string.IsNullOrEmpty(request.FingerPrintData)
                        ? "EmployeeIdAndFingerPrint"
                        : (request.EmployeeId.HasValue ? "EmployeeId" : "FingerPrint");

                    // Find the user based on EmployeeId or FingerPrintData
                    var user = await _context.MasterTable
                        .FirstOrDefaultAsync(u =>
                            u.EmployeeId == request.EmployeeId ||
                            u.FingerPrintData == request.FingerPrintData);

                    if (user == null)
                    {
                        // If user not found, add an error response and continue
                        responses.Add(new AttendanceResponse
                        {
                            EmployeeId = request.EmployeeId ?? 0,
                            Status = "Error",
                            Operation = request.Op == 1 ? "Check-In" : "Check-Out",
                            OpDateTime = request.OpDateTime,
                            Id = 0,
                            
                        });
                        continue;
                    }

                    // Create a new transaction
                    var transaction = new TransactionTable
                    {
                        MasterId = user.MasterId,
                        OpDateTime = request.OpDateTime,
                        Op = request.Op,
                        CheckInMethod = checkInMethod,
                        LatestTransactionDate = DateTime.UtcNow
                    };

                    _context.TransactionTable.Add(transaction);

                    // Add a successful response
                    responses.Add(new AttendanceResponse
                    {
                        Id = transaction.Id,
                        EmployeeId = user.EmployeeId,
                        Operation = request.Op == 1 ? "Check-In" : "Check-Out",
                        OpDateTime = transaction.OpDateTime,
                        Status = "Success"
                    });
                }
                catch (Exception ex)
                {
                    // Add an error response for exceptions
                    responses.Add(new AttendanceResponse
                    {
                        EmployeeId = request.EmployeeId ?? 0,
                        Status = "Error",
                        Operation = request.Op == 1 ? "Check-In" : "Check-Out",
                        OpDateTime = request.OpDateTime,
                        Id = 0,
                        
                    });
                }
            }

            // Save all transactions to the database
            await _context.SaveChangesAsync();

            return Ok(responses);
        }

 //        //code updated
 //         public async Task<IActionResult> ManageAttendance([FromBody] List<AttendanceRequest> requests)
 // {
 //     if (requests == null || requests.Count == 0)
 //     {
 //         return BadRequest("Request list cannot be empty.");
 //     }

 //     var responses = new List<AttendanceResponse>();

 //     foreach (var request in requests)
 //     {
 //         try
 //         {
 //             // Validate input: Either EmployeeId or FingerPrintData must be provided
 //             if (!request.EmployeeId.HasValue && string.IsNullOrEmpty(request.FingerPrintData))
 //             {
 //                 responses.Add(new AttendanceResponse
 //                 {
 //                     Status = "Error",
 //                     Operation = request.Op == 1 ? "Check-In" : "Check-Out",
 //                     OpDateTime = request.OpDateTime,
 //                     Id = 0,
 //                    //CheckInMethod = "Invalid Identification"
 //                 });
 //                 continue;
 //             }

 //             // Find the user
 //             MasterTable user = null;
 //             if (request.EmployeeId.HasValue)
 //             {
 //                 user = await _context.MasterTable
 //                     .FirstOrDefaultAsync(u => u.EmployeeId == request.EmployeeId.Value);
 //             }

 //             if (user == null && !string.IsNullOrEmpty(request.FingerPrintData))
 //             {
 //                 user = await _context.MasterTable
 //                     .FirstOrDefaultAsync(u => u.FingerPrintData == request.FingerPrintData);
 //             }

 //             //if (user == null)
 //             //{
 //             //    responses.Add(new AttendanceResponse
 //             //    {
 //             //        EmployeeId = request.EmployeeId ?? 0,
 //             //        Status = "Error",
 //             //        Operation = request.Op == 1 ? "Check-In" : "Check-Out",
 //             //        OpDateTime = request.OpDateTime,
 //             //        Id = 0,
 //             //        CheckInMethod = request.EmployeeId.HasValue ? "EmployeeId" : "FingerPrint"
 //             //    });
 //             //    continue;
 //             //}

 //             // Determine check-in method
 //             string checkInMethod = request.EmployeeId.HasValue && !string.IsNullOrEmpty(request.FingerPrintData)
 //                 ? "EmployeeIdAndFingerPrint"
 //                 : (request.EmployeeId.HasValue ? "EmployeeId" : "FingerPrint");

 //             // Handle Check-In
 //             if (request.Op == 1)
 //             {
 //                 var lastTransaction = await _context.TransactionTable
 //                     .Where(t => t.MasterId == user.MasterId)
 //                     .OrderByDescending(t => t.Id)
 //                     .FirstOrDefaultAsync();

 //                 if (lastTransaction != null && lastTransaction.Op == 1)
 //                 {
 //                     responses.Add(new AttendanceResponse
 //                     {
 //                         EmployeeId = user.EmployeeId,
 //                         Status = "Error",
 //                         Operation = "Check-In",
 //                         OpDateTime = request.OpDateTime,
 //                         Id = 0,
 //                         //CheckInMethod = "Already Checked In Without Checkout"
 //                     });
 //                     continue;
 //                 }

 //                 var transaction = new TransactionTable
 //                 {
 //                     MasterId = user.MasterId,
 //                     OpDateTime = request.OpDateTime,
 //                     Op = 1,
 //                     CheckInMethod = checkInMethod,
 //                     LatestTransactionDate = DateTime.UtcNow
 //                 };

 //                 _context.TransactionTable.Add(transaction);
 //                 user.LastTransactionDate = transaction.OpDateTime;

 //                 responses.Add(new AttendanceResponse
 //                 {
 //                     Id = transaction.Id,
 //                     EmployeeId = user.EmployeeId,
 //                     Operation = "Check-In",
 //                     OpDateTime = transaction.OpDateTime,
 //                    // CheckInMethod = transaction.CheckInMethod,
 //                     Status = "Success"
 //                 });
 //             }
 //             // Handle Check-Out
 //             else if (request.Op == 0)
 //             {
 //                 var lastCheckIn = await _context.TransactionTable
 //                     .Where(t => t.MasterId == user.MasterId && t.Op == 1)
 //                     .OrderByDescending(t => t.Id)
 //                     .FirstOrDefaultAsync();

 //                 //if (lastCheckIn == null)
 //                 //{
 //                 //    responses.Add(new AttendanceResponse
 //                 //    {
 //                 //        EmployeeId = user.EmployeeId,
 //                 //        Status = "Error",
 //                 //        Operation = "Check-Out",
 //                 //        OpDateTime = request.OpDateTime,
 //                 //        Id = 0,
 //                 //        CheckInMethod = "No Active Check-In"
 //                 //    });
 //                 //    continue;
 //                 //}

 //                 var transaction = new TransactionTable
 //                 {
 //                     MasterId = user.MasterId,
 //                     OpDateTime = request.OpDateTime,
 //                     Op = 0,
 //                     CheckInMethod = checkInMethod,
 //                     LatestTransactionDate = DateTime.UtcNow
 //                 };

 //                 _context.TransactionTable.Add(transaction);

 //                 responses.Add(new AttendanceResponse
 //                 {
 //                     Id = transaction.Id,
 //                     EmployeeId = user.EmployeeId,
 //                     Operation = "Check-Out",
 //                     OpDateTime = transaction.OpDateTime,
 //                    //CheckInMethod = transaction.CheckInMethod,
 //                     Status = "Success"
 //                 });
 //             }
 //             else
 //             {
 //                 responses.Add(new AttendanceResponse
 //                 {
 //                     EmployeeId = request.EmployeeId ?? 0,
 //                     Status = "Error",
 //                     Operation = "Invalid Operation",
 //                     OpDateTime = request.OpDateTime,
 //                     Id = 0
 //                 });
 //             }
 //         }
 //         catch (Exception ex)
 //         {
 //             responses.Add(new AttendanceResponse
 //             {
 //                 EmployeeId = request.EmployeeId ?? 0,
 //                 Status = "Error",
 //                 Operation = request.Op == 1 ? "Check-In" : "Check-Out",
 //                 OpDateTime = request.OpDateTime,
 //                 Id = 0,
 //                 //CheckInMethod = "Exception Occurred"
 //             });
 //         }
 //     }

 //     // Save all changes after processing the requests
 //     await _context.SaveChangesAsync();

 //     return Ok(responses);
 // } 








        /*[HttpGet("transactions/{employeeId}")]
        public async Task<IActionResult> GetTransactionDataById(int employeeId)
        {
            try
            {
                var master = await _context.MasterTable
                    .Where(m => m.EmployeeId == employeeId)
                    .Select(m => new { m.MasterId, m.Employeename })
                    .FirstOrDefaultAsync();

                if (master == null)
                {
                    return NotFound($"Employee with EmployeeId {employeeId} not found.");
                }

                // First get the raw data
                var transactionData = await _context.TransactionTable
                    .Where(t => t.MasterId == master.MasterId)
                    .GroupBy(t => t.OpDateTime.Date)
                    .Select(g => new
                    {
                        date = g.Key,
                        checkIns = g.Where(t => t.Op == 1)
                                   .Select(t => t.OpDateTime)
                                   .Distinct(),
                        checkOuts = g.Where(t => t.Op == 0)
                                    .Select(t => t.OpDateTime)
                                    .Distinct()
                    })
                    .OrderByDescending(t => t.date)
                    .ToListAsync();

                if (!transactionData.Any())
                {
                    return NotFound($"No transactions found for EmployeeId {employeeId}");
                }

                // Then format the data after getting it from the database
                var result = new
                {
                    employeename = master.Employeename,
                    transactions = transactionData.Select(t => new
                    {
                        date = t.date.ToString("yyyy-MM-dd"),
                        checkIns = t.checkIns.OrderBy(d => d).ToList(),
                        checkOuts = t.checkOuts.OrderBy(d => d).ToList()
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        */


        //here added timezone
        /* [HttpGet("transactions/{employeeId}")]
         public async Task<IActionResult> GetTransactionDataById(int employeeId)
         {
             try
             {
                 var master = await _context.MasterTable
                     .Where(m => m.EmployeeId == employeeId)
                     .Select(m => new { m.MasterId, m.Employeename })
                     .FirstOrDefaultAsync();

                 if (master == null)
                 {
                     return NotFound($"Employee with EmployeeId {employeeId} not found.");
                 }

                 // Get the raw data and order by date descending (latest first)
                 var transactionData = await _context.TransactionTable
                     .Where(t => t.MasterId == master.MasterId)
                     .GroupBy(t => t.OpDateTime.Date)
                     .OrderByDescending(g => g.Key)  // Order groups by date descending
                     .Select(g => new
                     {
                         date = g.Key,
                         records = g.OrderBy(t => t.OpDateTime).ToList()
                     })
                     .ToListAsync();

                 if (!transactionData.Any())
                 {
                     return NotFound($"No transactions found for EmployeeId {employeeId}");
                 }

                 // Format the data
                 var result = new
                 {
                     employeename = master.Employeename,
                     visitHistory = transactionData.Select(t => new
                     {
                         date = t.date.ToString("d MMM yyyy"),
                         times = GetPairedTimes(t.records)
                     }).ToList()
                 };

                 return Ok(result);
             }
             catch (Exception ex)
             {
                 return StatusCode(500, $"Internal server error: {ex.Message}");
             }
         }

         private List<object> GetPairedTimes(List<TransactionTable> records)
         {
             var pairedTimes = new List<object>();

             for (int i = 0; i < records.Count; i++)
             {
                 if (records[i].Op == 1) // Check-in
                 {
                     var checkOutTime = records
                         .Skip(i + 1)
                         .FirstOrDefault(r => r.Op == 0)?.OpDateTime;

                     if (checkOutTime.HasValue)
                     {
                         pairedTimes.Add(new
                         {
                             checkInTime = records[i].OpDateTime.ToString("h:mm tt"),
                             checkOutTime = checkOutTime.Value.ToString("h:mm tt")
                         });
                     }
                 }
             }

             return pairedTimes;
         } */


        [HttpGet("transactions/{employeeId}")]
        public async Task<IActionResult> GetTransactionDataById(int employeeId)
        {
            try
            {
                // Fetch master record
                var master = await _context.MasterTable
                    .Where(m => m.EmployeeId == employeeId)
                    .Select(m => new { m.MasterId, m.Employeename })
                    .FirstOrDefaultAsync();

                if (master == null)
                {
                    return NotFound($"Employee with EmployeeId {employeeId} not found.");
                }

                // Fetch transaction data and group by date
                var rawTransactionData = await _context.TransactionTable
                    .Where(t => t.MasterId == master.MasterId)
                    .OrderBy(t => t.OpDateTime)
                    .ToListAsync();

                // Convert raw data into grouped, paired format
                var transactionData = rawTransactionData
                    .GroupBy(t => t.OpDateTime.Date)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new
                    {
                        date = g.Key.ToString("d MMM yyyy"),
                        times = GetPairedTimes(g.ToList())
                    })
                    .ToList();

                if (!transactionData.Any())
                {
                    return NotFound($"No transactions found for EmployeeId {employeeId}");
                }

                // Format the final result
                var result = new
                {
                    employeename = master.Employeename,
                    visitHistory = transactionData
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /*private List<object> GetPairedTimes(List<TransactionTable> records)
        {
            var pairedTimes = new List<object>();

            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].Op == 1) // Check-in
                {
                    // Find the next check-out (if it exists)
                    var checkOutTime = records
                        .Skip(i + 1)
                        .FirstOrDefault(r => r.Op == 0)?.OpDateTime;

                    if (checkOutTime.HasValue)
                    {
                        pairedTimes.Add(new
                        {
                            checkInTime = records[i].OpDateTime.ToString("h:mm tt"),
                            checkOutTime = checkOutTime.Value.ToString("h:mm tt")
                        });

                        // Skip the matched check-out record
                        i = records.IndexOf(records.First(r => r.OpDateTime == checkOutTime.Value));
                    }
                    else
                    {
                        // Include unmatched check-in
                        pairedTimes.Add(new
                        {
                            checkInTime = records[i].OpDateTime.ToString("h:mm tt"),
                            checkOutTime = (string)null
                        });
                    }
                }
                else if (records[i].Op == 0) // Unmatched check-out
                {
                    pairedTimes.Add(new
                    {
                        checkInTime = (string)null,
                        checkOutTime = records[i].OpDateTime.ToString("h:mm tt")
                    });
                }
            }

            return pairedTimes;
        }
        */

        private List<object> GetPairedTimes(List<TransactionTable> records)
        {
            var pairedTimes = new List<object>();
            bool[] processed = new bool[records.Count]; // Track processed records

            for (int i = 0; i < records.Count; i++)
            {
                if (processed[i]) continue; // Skip already processed records

                if (records[i].Op == 1) // Check-in
                {
                    // Find the next unprocessed check-out (if it exists)
                    var checkOutIndex = records
                        .FindIndex(i + 1, r => r.Op == 0 && !processed[records.IndexOf(r)]);

                    if (checkOutIndex != -1) // Matching check-out found
                    {
                        pairedTimes.Add(new
                        {
                            checkInTime = records[i].OpDateTime.ToString("h:mm tt"),
                            checkOutTime = records[checkOutIndex].OpDateTime.ToString("h:mm tt")
                        });

                        // Mark both check-in and check-out as processed
                        processed[i] = true;
                        processed[checkOutIndex] = true;
                    }
                    else
                    {
                        // Include unmatched check-in
                        pairedTimes.Add(new
                        {
                            checkInTime = records[i].OpDateTime.ToString("h:mm tt"),
                            checkOutTime = (string)null
                        });

                        processed[i] = true;
                    }
                }
                else if (records[i].Op == 0) // Unmatched check-out
                {
                    pairedTimes.Add(new
                    {
                        checkInTime = (string)null,
                        checkOutTime = records[i].OpDateTime.ToString("h:mm tt")
                    });

                    processed[i] = true;
                }
            }

            return pairedTimes;
        }






    }


}
