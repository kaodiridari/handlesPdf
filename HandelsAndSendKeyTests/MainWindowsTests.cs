using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Tests
{   class QuasiLogger
    {
        static string path = @"C:\Users\user\Documents\Visual Studio 2015\Projects\handlesPdf\HandelsAndSendKeyTests\files\log.log";

        public static void clear()
        {
            try
            {
                File.Delete(path);
            } catch (Exception e)
            {
                //ignore -maybe opened somewhere
                log("Can't delete this file");
            }
            
        }

        public static void log(string txt)
        {
            txt = txt + "\n";
            using (FileStream fs = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(txt);
                fs.Write(info, 0, info.Length);
            }
        } 
    }

    [TestClass()]
    public class MainWindowsTests
    {
        int _calls = 0;
        DateTime timeLastCall;
        private static readonly double cycle = 1500.0;   //time betwween callbacks in millis

        private static System.Timers.Timer aTimer;
        private static readonly double _timeoutForTest = 15000;  //Test should end after this timespan. This means success. 

        private static System.Timers.Timer quasiUserTimer;
        private static readonly double _timequasiUserTimer = cycle * 2;

        private MainWindows _mw;
        private bool _finished = false;
        private bool _hasException;
        private Exception _ex;

        private readonly long _startUnixMilliSeconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private long unixMilliSeconds;
        private int quasiUserTimerCalls = -1;

        [TestInitialize]
        public void Setup()
        {
            QuasiLogger.clear();
            unixMilliSeconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        [TestCleanup]
        public void Cleanup()
        {
            QuasiLogger.log("cleaningup...");
            //mw.stop();
        }

        //private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        [TestMethod()]
        public void MainWindowsTest()
        {
            QuasiLogger.log("MainWindowsTest()");
            MainWindows.MainWindowsDelegate x = new MainWindows.MainWindowsDelegate(blah);
            List<MainWindows.MainWindowsDelegate> clients = new List<MainWindows.MainWindowsDelegate>(1);
            clients.Add(x);
            int i = 1000;
            List<FakeWindowStuff.FakedWindow> fws = new List<FakeWindowStuff.FakedWindow>();
            fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_1", false));   //1000
            fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_2", true));
            fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_3", false));
            fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_4", true));
            fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_5", false));
            fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_6", true));
            fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_7", false));
            fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_8", true));

            //SetTimer for ending the test;
            QuasiLogger.log("SetTimer()");
            aTimer = new System.Timers.Timer(_timeoutForTest);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            //set windows first time
            _mw = new MainWindows(Convert.ToInt32(cycle), clients, new FakeWindowStuff(fws));

            //a timer after n seconds which alters after a pause the window list ... = user did something.
            QuasiLogger.log("setting usertimer to: " + _timequasiUserTimer + " mytime: " + myTime());
            quasiUserTimer = new System.Timers.Timer(_timequasiUserTimer);
            quasiUserTimer.Elapsed += OnquasiUserTimer;
            quasiUserTimer.AutoReset = true;
            quasiUserTimer.Enabled = true;

            //test leider beendet...
            while (!_finished && !_hasException)
            {

            }
            if (_hasException)
                Assert.Fail("An exception occured.");
            if (_calls != 4)
                Assert.Fail("Not all tests done.");        
        }

        

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            QuasiLogger.log("OnTimedEvent() " + myTime());
            _mw.stop();
            _finished = true;
            //Assert.Fail();
            //throw new TimeoutException();
            //Environment.Exit(4711);
        }

        //alle 3 sek.
        private void OnquasiUserTimer(object source, ElapsedEventArgs e)
        {
            quasiUserTimerCalls++;
            QuasiLogger.log("OnquasiUserTimer() " + myTime() + " quasiUserTimerCalls " + quasiUserTimerCalls);

            switch (quasiUserTimerCalls)
            {
                case 0:    //second test, first test was set by constructor
                    {
                        //prepare next test: 1003 vanished, 4711 new
                        int i = 1000;
                        List<FakeWindowStuff.FakedWindow> fws = new List<FakeWindowStuff.FakedWindow>();
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_1", false));   //1000
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_2", true));
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_3", false));
                        i++; //1003 vanished
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_5", false));
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_6", true));
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_7", false));
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(i++), "TestWindow_8", true));
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(4711), "TestWindow_4711", true));
                        FakeWindowStuff f = new FakeWindowStuff(fws);
                        setFakeWindows(f);
                        break;
                    }
                case 1:
                    {
                        //prepare next test
                        List<FakeWindowStuff.FakedWindow> fws = new List<FakeWindowStuff.FakedWindow>();
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(1005), "Last Window", true));
                        FakeWindowStuff f = new FakeWindowStuff(fws);
                        setFakeWindows(f);

                        QuasiLogger.log("set window 1005. All other should vanish.");
                        break;
                    }
                case 2:
                //case 3:
                    {
                        QuasiLogger.log("empty cycle " + quasiUserTimerCalls);
                        break;
                    }
                case 3:
                    {
                        //A new Window has been created.
                        List<FakeWindowStuff.FakedWindow> fws = new List<FakeWindowStuff.FakedWindow>();
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(1005), "Last Window", true));
                        fws.Add(new FakeWindowStuff.FakedWindow(new IntPtr(1234), "Newer Window", true));
                        FakeWindowStuff f = new FakeWindowStuff(fws);
                        setFakeWindows(f);

                        QuasiLogger.log("set window 1234 and existing one 1005.");
                        break;
                    }
                default:
                    {
                        QuasiLogger.log("default");
                        break;
                    }
            }
        }

        private void setFakeWindows(FakeWindowStuff f)
        {
            QuasiLogger.log("setFakeWindows " + myTime());
            try
            {
                while (_mw.isBusy())
                {
                    QuasiLogger.log("Target is busy. I pause " + myTime());
                    Thread.Sleep(100);
                }
                _mw.setWindowStuff(f);  //Fehler
            }
            catch (Exception ee)
            {
                QuasiLogger.log(ee.ToString());
                Assert.Fail();
            }
        }

        private long myTime()
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long t = Math.Abs(this._startUnixMilliSeconds - now);
            return t;
        } 
        
        private void blah(List<MainWindows.MainWindow> mws)
        {
            _calls++;
            QuasiLogger.log("blah called mws.Count = " + mws.Count);
            try
            { 
                switch (_calls)
                {
                    //First only visible windows 
                    case 1:      //0 ms
                        {
                            QuasiLogger.log("calls 1 " + myTime());
                            Assert.AreEqual(mws.Count, 4);
                            //expected handles - four visible windows.
                            List<IntPtr> handles = new List<IntPtr> { (IntPtr)1001, (IntPtr)1003, (IntPtr)1005, (IntPtr)1007 };
                            //log
                            foreach (MainWindows.MainWindow w in mws)
                            {
                                QuasiLogger.log("delivered handle: " + w.hWnd.ToString() + " isVisible, title: " + w.title);
                            }
                            foreach (MainWindows.MainWindow w in mws)
                            {
                                QuasiLogger.log("w.hwnd = " + w.hWnd + " " + handles.Contains(w.hWnd));
                                Assert.IsTrue(handles.Contains(w.hWnd));  
                                handles.Remove(w.hWnd);
                            }                            
                        }
                        break;
                    //second: one window is added, a second is deleted; 5 windows expected (see case 1)
                    case 2:      //3000ms
                        {
                            QuasiLogger.log("calls 2 " + myTime());
                            QuasiLogger.log("n windows: " + mws.Count.ToString());
                            Assert.AreEqual(mws.Count, 5);
                            foreach (MainWindows.MainWindow w in mws)
                            {
                                QuasiLogger.log("delivered handle: " + w.hWnd.ToString() + " title: " + w.title + " flag: " + w.flag);
                            }
                            //expected handles:
                            List<IntPtr> handles = new List<IntPtr> { (IntPtr)1001, (IntPtr)1003, (IntPtr)1005, (IntPtr)1007, (IntPtr)4711 };

                            //check the vanished window 1003
                            //var matches = myList.Where(p => p.Name == nameToExtract);
                            var matches = mws.Where(p => p.hWnd.Equals(new IntPtr(1003)));
                            QuasiLogger.log("n matches: " + matches.Count());
                            Assert.IsTrue(matches.Count() == 1);
                            MainWindows.MainWindow mw = (MainWindows.MainWindow)matches.First();
                            QuasiLogger.log("mw.flag: " + mw.flag);
                            Assert.IsTrue(mw.flag == MainWindows.MainWindow.flags.VANISHED);

                            //check the new window
                            matches = mws.Where(p => p.hWnd.Equals(new IntPtr(4711)));
                            QuasiLogger.log("matches for 4711: " + matches.Count());
                            Assert.IsTrue(matches.Count() == 1);
                            mw = (MainWindows.MainWindow)matches.First();
                            Assert.IsTrue(mw.flag == MainWindows.MainWindow.flags.NEW);

                            //check the rest
                            QuasiLogger.log("check the rest");
                            List<IntPtr> same = new List<IntPtr> { new IntPtr(1001), new IntPtr(1005), new IntPtr(1007) };

                            foreach (IntPtr aHandle in same)
                            {
                                matches = mws.Where(p => p.hWnd.Equals(aHandle));
                                Assert.IsTrue(matches.Count() == 1);
                                mw = (MainWindows.MainWindow)matches.First();
                                Assert.IsTrue(mw.flag == MainWindows.MainWindow.flags.SAME);
                            }
                            
                            timeLastCall = DateTime.Now;
                            unixMilliSeconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            QuasiLogger.log("unixSeconds " + unixMilliSeconds);

                            //we miss a cycle then the windows are changed.
                        }
                        break;
                    //we wait a cycle nothing should be done. we are not even called.
                    //All windows are gone except of the one and only last standing window out there: number 1005.
                    case 3:
                        {
                            QuasiLogger.log("calls 3 " + myTime());
                            int vanishedWindows = 0;
                            int sameWindows = 0;
                            //three vanished and one stayed
                            foreach (MainWindows.MainWindow w in mws)
                            {
                                QuasiLogger.log("delivered handle: " + w.hWnd.ToString() + " title: " + w.title + " flag: " + w.flag);
                                if (w.flag.Equals(MainWindows.MainWindow.flags.SAME))
                                {
                                    sameWindows++;
                                } else if (w.flag.Equals(MainWindows.MainWindow.flags.VANISHED))
                                {
                                    vanishedWindows++;
                                } else
                                {
                                    Assert.Fail(w.flag + " shouldn't be here.");
                                }
                            }
                            Assert.IsTrue(vanishedWindows == 3);
                            Assert.IsTrue(sameWindows == 1);
                            QuasiLogger.log("myTime " + myTime());
                            
                            //Assert.IsTrue(elapsed > cycle * 2 - 100 && elapsed < cycle * 2 + 100);
                        }
                        break;
                    case 4:
                        {
                            QuasiLogger.log("calls " + _calls + " " + myTime());
                            int vanishedWindows = 0;
                            int sameWindows = 0;
                            int newWindows = 0;
                            //none vanished one same and a new one
                            foreach (MainWindows.MainWindow w in mws)
                            {
                                QuasiLogger.log("delivered handle: " + w.hWnd.ToString() + " title: " + w.title + " flag: " + w.flag);
                                if (w.flag.Equals(MainWindows.MainWindow.flags.SAME))
                                {
                                    sameWindows++;
                                }
                                else if (w.flag.Equals(MainWindows.MainWindow.flags.VANISHED))
                                {
                                    vanishedWindows++;
                                }
                                else if (w.flag.Equals(MainWindows.MainWindow.flags.NEW))
                                {
                                    newWindows++;
                                }
                                else
                                {
                                    Assert.Fail(w.flag + " shouldn't be here.");
                                }                                 
                            }
                            Assert.IsTrue(vanishedWindows == 0);
                            Assert.IsTrue(sameWindows == 1);
                            Assert.IsTrue(newWindows == 1);
                            break;
                        }
                    default:
                        {
                            QuasiLogger.log("default");
                            long elapsed = DateTimeOffset.Now.ToUnixTimeMilliseconds() - unixMilliSeconds;
                            QuasiLogger.log("calls 3 time elapsed: " + elapsed);
                            QuasiLogger.log("calls " + _calls);
                            foreach (MainWindows.MainWindow w in mws)
                            {
                                QuasiLogger.log("delivered handle: " + w.hWnd.ToString() + " title: " + w.title + " flag: " + w.flag);
                            }
                            Assert.Fail("This should never be called.");
                        }
                        break;
                }
            } catch (Exception ex)
            {
                _ex = ex;
                Console.Write(ex + "\n");
                QuasiLogger.log(ex.StackTrace);
                _hasException = true;
                throw ex;
            } 
        }
    }
}