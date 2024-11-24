using System.Globalization;
using Csv;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

string login = default, password = default, csvPath = default;
float extraPoint = default;

Console.Write("Login: ");
login = Console.ReadLine();
Console.Write("Password: ");
password = Console.ReadLine();
Console.Write("Extra point (default zero): ");
float.TryParse(Console.ReadLine(), out extraPoint);
Console.Write("CSV Path: ");
csvPath = Console.ReadLine();
var csv = File.ReadAllText(csvPath);

using (IWebDriver driver = new ChromeDriver())
{
    driver.Navigate().GoToUrl("https://portal.catolicasc.org.br/Corpore.Net/Login.aspx?autoload=false");
    driver.Manage().Window.Size = new System.Drawing.Size(974, 1040);
    driver.FindElement(By.Id("txtUser")).Click();
    driver.FindElement(By.Id("txtUser")).SendKeys(login);
    driver.FindElement(By.Id("txtPass")).SendKeys(password);
    driver.FindElement(By.Id("divLogin")).Click();
    driver.FindElement(By.Id("btnLogin")).Click();
    driver.FindElement(By.LinkText("Diário de classe")).Click();
   
    Console.WriteLine("Navegue até a página para digitação das notas dos alunos e pressione <ENTER> neste console.");
    Console.ReadLine();

    foreach (var line in CsvReader.ReadFromText(csv))
    {
        string studentId = default;
        float percent = default;

        try
        {
            studentId = line["Student ID"];
            percent = float.Parse(line["Percent Correct"], CultureInfo.GetCultureInfo("en-US"))/10;
            percent += extraPoint;

            IWebElement input = driver.FindElement(By.XPath("//input[starts-with(@id, 'tbProva_1_" + studentId + "')]"));
            Console.WriteLine(input.GetAttribute("id") + "\n" + percent.ToString("0.0"));
            input.SendKeys(percent.ToString("0.0"));
        } 
        catch (Exception)
        {
            Console.WriteLine("Erro ao inserir a nota do aluno " + studentId + " " + percent);
        }
    }

    Console.WriteLine("Verifique as notas e salve.");
    Console.ReadLine();
}