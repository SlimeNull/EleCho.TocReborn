using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Runtime.Caching;

namespace NullLib.TocReborn.Pro
{

    public class TocProServer
    {
        private readonly TocProListener baseListener;
        public TocListener BaseListener => baseListener;

        bool loopAccept;
        Task? acceptTask;

        public Dictionary<TocProConnection, TocProUser?> Clients { get; } = new Dictionary<TocProConnection, TocProUser?>();
        public ITocProServerDataManager DataSolver { get; }

        public MemoryCache accountRegisterCache = new MemoryCache(nameof(accountRegisterCache));


        public TocProServer(TocProListener listener, ITocProServerDataManager storage)
        {
            baseListener = listener;
            DataSolver = storage;
        }

        internal async Task<TocProConnection> InternalAcceptClientAsync()
        {
            return await baseListener.AcceptTocProConnectionAsync();
        }

        async Task AcceptConnectionLoop()
        {
            while (loopAccept)
            {
                TocProConnection conn = await InternalAcceptClientAsync();
                OnClientAccepted(conn);
            }
        }

        public void Start()
        {
            loopAccept = true;
            baseListener.Start();
            acceptTask = AcceptConnectionLoop();
        }

        public void Stop()
        {
            loopAccept = false;
            baseListener.Stop();
            if (acceptTask != null)
                acceptTask.Wait();
        }

        private TocProMessage? ProcClientInteract(TocProConnection conn, TocProMessage? proMessage)
        {
            return proMessage switch
            {
                TocProAccountCreateInteractMessage msg => ProcClientAccountCreateMsg(conn, msg),
                TocProAccountVerifyInteractMessage msg => ProcClientAccountVerifyMsg(conn, msg),
                TocProAccountDeleteInteractRequest msg => ProcClientAccountDeleteMsg(conn, msg),
                TocProAccountLoginInteractMessage msg => ProcClientAccountLoginMsg(conn, msg),

                TocProChannelCreateInteractMessage msg => ProcClientChannelCreateMsg(conn, msg),
                TocProChannelDeleteInteractMessage msg => ProcClientChannelDeleteMsg(conn, msg),

                _ => null,
            };
        }

        private TocProMessage? ProcClientChannelDeleteMsg(TocProConnection conn, TocProChannelDeleteInteractMessage msg) => throw new NotImplementedException();
        private TocProMessage? ProcClientChannelCreateMsg(TocProConnection conn, TocProChannelCreateInteractMessage msg) => throw new NotImplementedException();
        private TocProMessage? ProcClientAccountLoginMsg(TocProConnection conn, TocProAccountLoginInteractMessage msg)
        {
            TocProUser? account = DataSolver.Users
                .Where(v => v.Account == msg.Account).FirstOrDefault();

            if (account != null)
            {
                if (account.Password == msg.Password)
                {
                    Clients[conn] = account;
                    return new TocProEmptyInteractMessage()
                    {
                        Response = TocProMessageResponse.OK,
                    };
                }
                else
                {
                    return new TocProEmptyInteractMessage()
                    {
                        Response = new TocProMessageResponse(TocProAccountLoginInteractMessage.ErrorInvalidPassword, "InvalidPassword"),
                    };
                }
            }
            else
            {
                return new TocProEmptyInteractMessage()
                {
                    Response = new TocProMessageResponse(TocProAccountLoginInteractMessage.ErrorInvalidAccount, "InvalidAccount"),
                };
            }
        }
        private TocProMessage? ProcClientAccountDeleteMsg(TocProConnection conn, TocProAccountDeleteInteractRequest msg)
        {
            TocProUser? toRemove = DataSolver.Users.Where(a => a.Account == msg.Account).FirstOrDefault();
            if (toRemove != null)
            {
                DataSolver.Users.Remove(toRemove);
                return new TocProEmptyInteractMessage()
                {
                    Response = TocProMessageResponse.OK
                };
            }
            else
            {
                return new TocProEmptyInteractMessage()
                {
                    Response = new TocProMessageResponse(
                        TocProAccountDeleteInteractRequest.ErrorAccountNotExist,
                        "Account is not exist")
                };
            }
        }
        private TocProMessage? ProcClientAccountVerifyMsg(TocProConnection conn, TocProAccountVerifyInteractMessage msg)
        {
            if (accountRegisterCache.Contains(msg.Account))
            {
                string? account = msg.Account;
                string? verificationCode = accountRegisterCache[account] as string;

                if (msg.VerificationCode == verificationCode)
                {
                    DataSolver.Users.Add(new TocProUser(account!, msg.Password!));
                    return new TocProEmptyInteractMessage()
                    {
                        Response = TocProMessageResponse.OK
                    };
                }
                else
                {
                    return new TocProEmptyInteractMessage()
                    {
                        Response = new TocProMessageResponse(
                            TocProAccountVerifyInteractMessage.ErrorInvalidCode,
                            "Verification code is invalid")
                    };
                }
            }
            else
            {
                return new TocProEmptyInteractMessage()
                {
                    Response = new TocProMessageResponse(
                        TocProAccountVerifyInteractMessage.ErrorNotRequestedOrExpired,
                        "You hanvn't requested creating account or verification code is expired")
                };
            }
        }
        private TocProMessage? ProcClientAccountCreateMsg(TocProConnection conn, TocProAccountCreateInteractMessage msg)
        {
            string RandomDigits(int len)
            {
                var r  = new Random();
                char[] digits = new char[len];
                for (int i = 0; i < len; i++)
                {
                    digits[i] = (char)('0' + r.Next(0, 10));
                }
                return new string(digits);
            }

            string? NewAccount(int len, int tryTimes)
            {
                int tryedTimes = 0;
                while (tryedTimes < tryTimes)
                {
                    string account = RandomDigits(len);
                    if (!DataSolver.ContainsAccount(account))
                        return account;
                }

                return null;
            }

            string? newAccount = NewAccount(10, 5);
            string verificationCode = RandomDigits(6);

            if (newAccount != null && accountRegisterCache.Add(new CacheItem(newAccount, verificationCode), new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5),
            }))
            {
                try
                {
                    if (msg.EMail != null)
                    {
                        DataSolver.SendVerificationCode(msg.EMail, newAccount, verificationCode);
                        return new TocProAccountCreateInteractMessage()
                        {
                            Account = newAccount,
                            Response = TocProMessageResponse.OK
                        };
                    }
                    else
                    {
                        return new TocProEmptyInteractMessage()
                        {
                            Response = new TocProMessageResponse(
                                TocProAccountCreateInteractMessage.ErrorEmptyMailAddr,
                                "Invalid e-mail address")
                        };
                    }
                }
                catch
                {
                    return new TocProEmptyInteractMessage()
                    {
                        Response = new TocProMessageResponse(
                            TocProAccountCreateInteractMessage.ErrorCannotSendCode,
                            "Cannot send verification code")
                    };
                }
            }
            else
            {
                return new TocProEmptyInteractMessage()
                {
                    Response = new TocProMessageResponse(
                        TocProAccountCreateInteractMessage.ErrorCannotCreate,
                        "Cannot create account for you now. pleace contact adminstrator")
                };
            }
        }

        private void ProcClientMessage(object? sender, ProMessageEventArgs proMessage)
        {
            if (proMessage.Message == null)
                return;
            
            foreach (TocProConnection conn in Clients.Keys)
            {
                if (conn.IsConnected)
                {
                    conn.Send(proMessage.Message);
                }
            }
        }

        private void SubcribeClientEvents(TocProConnection conn)
        {
            conn.SubcribeInteract(ProcClientInteract);
            conn.ProMessageReceived += ProcClientMessage;
        }
        private void UnsubcribeClientEvents(TocProConnection conn)
        {
            conn.UnsubcribeInteract(ProcClientInteract);
            conn.ProMessageReceived -= ProcClientMessage;
        }

        public void OnClientAccepted(TocProConnection conn)
        {
            Clients[conn] = null;
            SubcribeClientEvents(conn);
            ClientAccepted?.Invoke(this, new ProConnectionEventArgs(conn));
        }

        public void OnClientClosed(TocProConnection conn)
        {
            Clients.Remove(conn);
            UnsubcribeClientEvents(conn);
            ClientClosed?.Invoke(this, new ProConnectionEventArgs(conn));
        }

        public event EventHandler<ProConnectionEventArgs>? ClientAccepted;
        public event EventHandler<ProConnectionEventArgs>? ClientClosed;
    }
}
