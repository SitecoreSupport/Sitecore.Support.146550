using Sitecore.Diagnostics;
using Sitecore.Modules.EmailCampaign.Core.Extensions;
using Sitecore.Modules.EmailCampaign.Core.Pipelines.GenerateLink;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using Sitecore.Modules.EmailCampaign.Core.Pipelines;
using Sitecore.EDS.Core.Dispatch;


namespace Sitecore.Support.EmailCampaign.Cm.Pipelines.SendEmail
{
  public class CheckIfLinkCorrect
  {
    public void Process(SendMessageArgs args)
    {
      Assert.IsNotNull(args, "Arguments can't be null");
      //string regexPattern = @"<a [^\s]*>";
      string regexPattern = @"<a([^>])+";
      EmailMessage emailMessage = args.CustomData["EmailMessage"] as EmailMessage;
      Regex regex = new Regex(regexPattern);
      Match match = regex.Match(emailMessage.HtmlBody);
      string result = String.Empty;
      string editResult = String.Empty;
      while (match.Success)
      {
        result = match.Value;
        if (!result.Contains("href"))
        {
          if (result.IndexOf("http") > -1)
          {
            Regex reg2 = new Regex(@"http[^\s]*\s");
            if (reg2.IsMatch(result))
            {
              Match match2 = reg2.Match(result);
              editResult = result.Replace(match2.Value, "href=\"" + match2.Value.Trim() + "\" ");
              emailMessage.HtmlBody = emailMessage.HtmlBody.Replace(result, editResult);
            }
            else
            {
              Regex reg3 = new Regex(@"http[^\s]*");
              Match match3 = reg3.Match(result);
              if (match3.Success)
              {
                string temp = match3.Value;
                editResult = result.Replace(temp, "href=\"" + temp.Trim() + "\" ");
                emailMessage.HtmlBody = emailMessage.HtmlBody.Replace(result, editResult);
              }
            }
          }
        }
        args.CustomData["EmailMessage"] = emailMessage;
        match = match.NextMatch();
      }
    }
  }
}