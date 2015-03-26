using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using remindsleepService.DataObjects;
using remindsleepService.Models;

namespace remindsleepService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            Database.SetInitializer(new remindsleepInitializer());
        }
    }

    public class remindsleepInitializer : ClearDatabaseSchemaIfModelChanges<remindsleepServiceContext>
    {
        protected override void Seed(remindsleepServiceContext context)
        {
            List<Member> members = new List<Member>
            {
                new Member { Id = Guid.NewGuid().ToString(),Name="msp的昌伟哥哥", Account="842146255@qq.com", Description="移动互联网菜鸟",Password="1145561428zcw" },
                new Member{Id=Guid.NewGuid().ToString(),Name ="msp穷穷的阿兜",Account="350970890@qq.com",Description="兜船长",Password="testpassword"}
                //new Member { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false },
            };

            foreach (Member item in members)
            {
                context.Set<Member>().Add(item);
            }

            List<Friend> friends = new List<Friend>
            {
                new Friend{ Id=Guid.NewGuid().ToString(),  MemberFirst=members[0].Id,MemberSecond=members[1].Id}
            };
            foreach (Friend item in friends)
            {
                context.Set<Friend>().Add(item);
            }
            List<Notification> notifications = new List<Notification>
            {
                new Notification{ Id=Guid.NewGuid().ToString(),SendId=members[0].Id,ReceiveId=members[1].Id,Title="早点休息",Content="熬夜对身体不好，晚安^_^", SendTime=DateTime.Now,IsCompleted=false}
            };
            foreach(Notification item in notifications){
                context.Set<Notification>().Add(item);
            }
          /*  List<TodoItem> todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false },
            };

            foreach (TodoItem todoItem in todoItems)
            {
                context.Set<TodoItem>().Add(todoItem);
            }*/
            base.Seed(context);
        }
    }
}

