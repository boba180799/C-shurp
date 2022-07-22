using OpenPop.Pop3;
namespace my_space
{
    class Program
    {

        /// <summary>
        /// проверка новых сообщений
        /// </summary>
        /// <param name="hostname">имя хоста  (пример: pop3.live.com)</param>
        /// <param name="port">номер порта ( 110 без SSL протокола, 995 с SSL протоколом POP3</param>
        /// <param name="useSsl">true если используем SSL протокол</param>
        /// <param name="username">имя пользователя для аутентификации</param>
        /// <param name="password">пароль пользователя для аутентификации</param>
        /// <param name="filename">путь к файлу для четения и записи новых и старых писем</param>

        public static void Check_mail(string username, string password, string hostname = "pop.mail.ru", int port = 995, bool useSsl = true, string filename = "path")
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            using (Pop3Client client = new Pop3Client())
            {
                try
                {
                    client.Connect(hostname, port, useSsl);
                    if(client.Connected)
                        client.Authenticate(username, password);
                    //получаем список старых писем из файла
                    List<string> oldMesUid = get_oldMesUid(filename);
                    //получаем все письма от сервера
                    List<string> newMesUid = client.GetMessageUids();
                    //псисок старых писем пуст, то выводим количество новых писем
                    if (oldMesUid.Count == 0)
                    {
                        Console.WriteLine(String.Format($"Количество новых сообщений {newMesUid.Count}"));
                        return;
                    }
                    //если список старых писем не пуст, то из нового списка удаляем старый список чтобы узнать количество новых сообщений
                    foreach (string str in oldMesUid)
                    {
                        if(newMesUid.Contains(str))
                            newMesUid.Remove(str);
                    }
                    Console.WriteLine(String.Format($"Количество новых сообщений {newMesUid.Count}"));
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format($"Exception: {e.Message}"));
                }
            }
        }
        /// <summary>
        /// метод считывания списка Uid старых писем из файла
        /// </summary>
        /// <param name="filename">путь к файлу из готорого будет считываться текст</param>
        /// <returns></returns>
        static public List<string> get_oldMesUid(string filename)
        {
            if(!File.Exists(filename))
                throw new FileNotFoundException("filename");


            List<string> oldMesUid = new List<string>();
            String line;
            try
            {
                StreamReader sr = new StreamReader(filename);
                line = sr.ReadLine();
                while (line != null)
                {
                    oldMesUid.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
                return oldMesUid;
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format($"Exception: {e.Message}"));
                return new List<string>();
            }
        }

        /// <summary>
        /// метод записи новых Uid писем в файл
        /// </summary>
        /// <param name="newMesUid">список новых писем которые просмотрены</param>
        /// <param name="filename">путь к файлу для записи</param>
        static public void set_newMesUid(List<string> newMesUid, string filename)
        {
            if (!Directory.Exists(filename))
                throw new DirectoryNotFoundException("filename");
            try
            {
                StreamWriter sw = new StreamWriter(filename);
                foreach (string i in newMesUid)
                    sw.WriteLine(i);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format($"Exception: {e.Message}"));
            }
        }

        static void Main(string[] args)
        {
            string login="login", password="password";
            Check_mail(login, password);
        }
    }
}

