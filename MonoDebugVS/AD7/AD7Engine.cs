using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugger.Soft;

namespace DynamicDevices.MonoDebugVS
{

    // AD7Engine is the primary entrypoint object for the sample engine. 
    //
    // It implements:
    //
    // IDebugEngine2: This interface represents a debug engine (DE). It is used to manage various aspects of a debugging session, 
    // from creating breakpoints to setting and clearing exceptions.
    //
    // IDebugEngineLaunch2: Used by a debug engine (DE) to launch and terminate programs.
    //
    // IDebugProgram3: This interface represents a program that is running in a process. Since this engine only debugs one process at a time and each 
    // process only contains one program, it is implemented on the engine.
    //
    // IDebugEngineProgram2: This interface provides simultanious debugging of multiple threads in a debuggee.

    [ComVisible(true)]
    [Guid("B759E695-A6DB-4A84-A316-85E104AC3B49")]
    public class AD7Engine : IDebugEngine2, IDebugEngineLaunch2, IDebugProgram3, IDebugEngineProgram2, IDebugSymbolSettings100
    {
        #region Fields

        /// <summary>
        /// ID of the debugger
        /// </summary>
        public const string Id = "{67e9d912-bd2d-49b7-9214-5d2595a39a5f}";

        private VirtualMachine _vm;

        private string _monoFilePath = "C:\\Program Files (x86)\\Mono-2.10.9\\bin\\mono";

        public EngineCallback Callback { get; set; }

        private Guid _programGuid = Guid.Empty;

        private Thread _monoMessageWorker;

        private MethodMirror _entryPoint;

        private BreakpointManager _breakpointManager;

        #endregion

        public AD7Engine()
        {
            _breakpointManager = new BreakpointManager(this);            
        }

        #region IDebugEngine2 Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rgpPrograms"></param>
        /// <param name="rgpProgramNodes"></param>
        /// <param name="celtPrograms"></param>
        /// <param name="pCallback"></param>
        /// <param name="dwReason"></param>
        /// <returns></returns>
        public int Attach(IDebugProgram2[] rgpPrograms, IDebugProgramNode2[] rgpProgramNodes, uint celtPrograms, IDebugEventCallback2 pCallback, enum_ATTACH_REASON dwReason)
        {
            IDebugProcess2 process;
            var pid = new AD_PROCESS_ID[1];

            // Get and strore our program GUID, for program under debug

            if (rgpPrograms[0].GetProcess(out process) != 0)
                throw new InvalidOperationException();

            if(process.GetPhysicalProcessId(pid) != 0)
                throw new InvalidOperationException();

            var processId = pid[0].dwProcessId;
            if (processId == 0)
            {
                return Constants.E_NOTIMPL; 
            }

            if(rgpPrograms[0].GetProgramId(out _programGuid) != 0)
                throw new InvalidOperationException();

            // Create message worker
            if (_monoMessageWorker == null)
            {
                _monoMessageWorker = new Thread(MonoMsgWorker) {IsBackground = true};
                _monoMessageWorker.Start();
            }
            // Now set up callback for event handling
            Callback = new EngineCallback(this, pCallback);

            // Send engine creation event (should possibly do this earlier)
            AD7EngineCreateEvent.Send(this);

            // Send program creation event
            AD7ProgramCreateEvent.Send(this);

            return Constants.S_OK;
        }

        private void MonoMsgWorker()
        {
            while(true)
            {
                var evtSet = _vm.GetNextEventSet();
                foreach(var evt in evtSet.Events)
                {
                    switch(evt.EventType)
                    {
                        case EventType.VMStart :
                            break;
                        case EventType.VMDisconnect:
                            break;
                        case EventType.VMDeath:
                            break;
                        case EventType.AssemblyLoad:
                            		var ae = (AssemblyLoadEvent)evt;
				                    _entryPoint = ae.Assembly.EntryPoint;
                                    break;
                            break;
                        case EventType.AssemblyUnload:
                            break;
                        case EventType.ThreadStart :
                            break;
                        case EventType.ThreadDeath:
                            break;
                        case EventType.Breakpoint:
                            break;
                        case EventType.Exception:
                            break;
                        case EventType.KeepAlive:
                            break;
                        case EventType.MethodEntry:
                            break;
                        case EventType.MethodExit:
                            break;
                        case EventType.Step:
                            break;
                        case EventType.TypeLoad:
                            break;
                        case EventType.AppDomainCreate:
                            break;
                        case EventType.AppDomainUnload:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public int CauseBreak()
        {
            throw new NotImplementedException();
        }                                                         

        public int ContinueFromSynchronousEvent(IDebugEvent2 pEvent)
        {
            throw new NotImplementedException();
        }

        public int CreatePendingBreakpoint(IDebugBreakpointRequest2 pBPRequest, out IDebugPendingBreakpoint2 ppPendingBP)
        {

            ppPendingBP = null;
            
            try
            {
               _breakpointManager.CreatePendingBreakpoint(pBPRequest, out ppPendingBP);
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }

            return Constants.S_OK;
        }

        public int DestroyProgram(IDebugProgram2 pProgram)
        {
            throw new NotImplementedException();
        }

        public int EnumPrograms(out IEnumDebugPrograms2 ppEnum)
        {
            Debug.Fail("This function is not called by the debugger");
            ppEnum = null;
            return Constants.E_NOTIMPL;
        }

        public int GetEngineId(out Guid pguidEngine)
        {
            pguidEngine = new Guid(Id);
            return Constants.S_OK;
        }

        public int RemoveAllSetExceptions(ref Guid guidType)
        {
            return Constants.S_OK;
        }

        public int RemoveSetException(EXCEPTION_INFO[] pException)
        {
            return Constants.S_OK;
        }

        public int SetException(EXCEPTION_INFO[] pException)
        {
            return Constants.S_OK;
        }

        public int SetLocale(ushort wLangID)
        {
            return Constants.S_OK;
        }

        public int SetMetric(string pszMetric, object varValue)
        {
            return Constants.S_OK;
        }

        public int SetRegistryRoot(string pszRegistryRoot)
        {
            return Constants.S_OK;
        }

        #endregion

        #region IDebugEngineLaunch2 Members

        public int CanTerminateProcess(IDebugProcess2 pProcess)
        {
            return Constants.S_OK;
        }

        /// <summary>
        /// Here is where we start off the process to be debugged
        /// </summary>
        /// <param name="pszServer"></param>
        /// <param name="pPort"></param>
        /// <param name="pszExe"></param>
        /// <param name="pszArgs"></param>
        /// <param name="pszDir"></param>
        /// <param name="bstrEnv"></param>
        /// <param name="pszOptions"></param>
        /// <param name="dwLaunchFlags"></param>
        /// <param name="hStdInput"></param>
        /// <param name="hStdOutput"></param>
        /// <param name="hStdError"></param>
        /// <param name="pCallback"></param>
        /// <param name="ppProcess"></param>
        /// <returns></returns>
        public int LaunchSuspended(string pszServer, IDebugPort2 pPort, string pszExe, string pszArgs, string pszDir, string bstrEnv, string pszOptions, enum_LAUNCH_FLAGS dwLaunchFlags, uint hStdInput, uint hStdOutput, uint hStdError, IDebugEventCallback2 pCallback, out IDebugProcess2 ppProcess)
        {
            // Deal with error on out (set below)
            ppProcess = null;

            try
            {
                var processStartInfo = new ProcessStartInfo(pszExe, pszArgs);

                // Make sure we use a custom launcher as the default filename used is the name of the exe
                // We need to wrap mono.exe around that exe...
                var opt = new LaunchOptions
                {
                    CustomProcessLauncher = LocalProcessLauncher
                };
 
                _vm = VirtualMachineManager.Launch(processStartInfo, opt);               
                //_vm.EnableEvents(new[] { EventType.VMDisconnect });
                
                var adProcessId = new AD_PROCESS_ID
                                      {
                                          ProcessIdType = (uint) enum_AD_PROCESS_ID.AD_PROCESS_ID_SYSTEM,
                                          dwProcessId = (uint) _vm.Process.Id
                                      };

                if (pPort.GetProcess(adProcessId, out ppProcess) != 0)
                    throw new InvalidOperationException("Couldn't setup process");

                // Store a new GUID (todo: How to get the real guid?)
                //pPort.GetPortId(out _programGuid);
//                _programGuid = Guid.NewGuid();

                // Send thread creation event
    //            var evt = new AD7ThreadCreateEvent();
  //              Callback.Send(evt, AD7ThreadCreateEvent.IID, this, null);

                // Load complete
  //              var evt2 = new AD7LoadCompleteEvent();
 //               Callback.Send(evt2, AD7LoadCompleteEvent.IID, this, null);

                return Constants.S_OK;
            }
            catch (Exception)
            {
                return Constants.RPC_E_SERVERFAULT;
            }
        }

        private Process LocalProcessLauncher(ProcessStartInfo info)
        {   
            // Modify process start info to use mono.exe
            var args = info.Arguments + " \"" + info.FileName + "\"";

            var process = Process.Start(_monoFilePath, args);

            return process;
        }

        public int ResumeProcess(IDebugProcess2 pProcess)
        {
            IDebugPort2 port;
            if(pProcess.GetPort(out port) != 0)
                throw new InvalidOperationException();

            var defaultPort = (IDebugDefaultPort2)port;

            IDebugPortNotify2 portNotify;
            if(defaultPort.GetPortNotify(out portNotify) != 0)
                throw new InvalidOperationException();

            if(portNotify.AddProgramNode(new AD7ProgramNode(_vm.Process.Id)) != 0)
                throw new InvalidOperationException();

            if (_programGuid  == Guid.Empty)
            {
                Debug.Fail("Unexpected problem -- IDebugEngine2.Attach wasn't called");
                return Constants.E_FAIL;
            }

#if false
            try
            {
                _vm.Resume();
            } catch
            {
            }
#endif

            return Constants.S_OK;
        }

        public int TerminateProcess(IDebugProcess2 pProcess)
        {
            try
            {
                _vm.Exit(0);
            } catch {}

            return Constants.S_OK;
        }

        #endregion

        #region IDebugProgram3 Members

        public int Attach(IDebugEventCallback2 pCallback)
        {
            Debug.Fail("This function is not called by the debugger");

            return Constants.E_NOTIMPL;
        }

        public int CanDetach()
        {
            return Constants.S_OK;
        }

        public int Continue(IDebugThread2 pThread)
        {
            throw new NotImplementedException();
        }

        public int Detach()
        {
            try
            {
            //    _vm.Exit(0);
            } catch
            {
            }

            return Constants.S_OK;
        }

        public int EnumCodeContexts(IDebugDocumentPosition2 pDocPos, out IEnumDebugCodeContexts2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int EnumCodePaths(string pszHint, IDebugCodeContext2 pStart, IDebugStackFrame2 pFrame, int fSource, out IEnumCodePaths2 ppEnum, out IDebugCodeContext2 ppSafety)
        {
            throw new NotImplementedException();
        }

        public int EnumModules(out IEnumDebugModules2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int EnumThreads(out IEnumDebugThreads2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int Execute()
        {
            Debug.Fail("This function is not called by the debugger");

            return Constants.E_NOTIMPL;
        }

        public int ExecuteOnThread(IDebugThread2 pThread)
        {
            throw new NotImplementedException();
        }

        public int GetDebugProperty(out IDebugProperty2 ppProperty)
        {
            throw new NotImplementedException();
        }

        public int GetDisassemblyStream(enum_DISASSEMBLY_STREAM_SCOPE dwScope, IDebugCodeContext2 pCodeContext, out IDebugDisassemblyStream2 ppDisassemblyStream)
        {
            throw new NotImplementedException();
        }

        public int GetENCUpdate(out object ppUpdate)
        {
            throw new NotImplementedException();
        }

        public int GetEngineInfo(out string pbstrEngine, out Guid pguidEngine)
        {
            throw new NotImplementedException();
        }

        public int GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
        {
            throw new NotImplementedException();
        }

        public int GetName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        public int GetProcess(out IDebugProcess2 ppProcess)
        {
            Debug.Fail("This function is not called by the debugger");
            ppProcess = null;
            return Constants.E_NOTIMPL;
        }

        public int GetProgramId(out Guid pguidProgramId)
        {
            pguidProgramId = _programGuid;

            return Constants.S_OK;
        }

        public int Step(IDebugThread2 pThread, enum_STEPKIND sk, enum_STEPUNIT Step)
        {
            throw new NotImplementedException();
        }

        public int Terminate()
        {
            throw new NotImplementedException();
        }

        public int WriteDump(enum_DUMPTYPE DUMPTYPE, string pszDumpUrl)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugEngineProgram2 Members

        public int Stop()
        {
            throw new NotImplementedException();
        }

        public int WatchForExpressionEvaluationOnThread(IDebugProgram2 pOriginatingProgram, uint dwTid, uint dwEvalFlags, IDebugEventCallback2 pExprCallback, int fWatch)
        {
            throw new NotImplementedException();
        }

        public int WatchForThreadStep(IDebugProgram2 pOriginatingProgram, uint dwTid, int fWatch, uint dwFrame)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugSymbolSettings100 Members

        public int SetSymbolLoadState(int bIsManual, int bLoadAdjacentSymbols, string bstrIncludeList, string bstrExcludeList)
        {
            return Constants.S_OK;
        }

        #endregion
    }
}