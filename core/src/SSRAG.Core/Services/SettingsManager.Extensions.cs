using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SSRAG.Configuration;
using SSRAG.Utils;
using Menu = SSRAG.Configuration.Menu;

namespace SSRAG.Core.Services
{
    public partial class SettingsManager
    {
        public List<Permission> GetPermissions()
        {
            var permissions = new List<Permission>();
            var section = Configuration.GetSection("extensions:permissions");
            if (!section.Exists()) return permissions;

            var list = section.Get<Dictionary<string, Permission>>();
            foreach (var (key, value) in list)
            {
                permissions.Add(new Permission
                {
                    Id = key,
                    Text = value.Text,
                    Type = value.Type,
                    Order = value.Order
                });
            }

            return permissions.OrderByDescending(x => x.Order.HasValue).ThenBy(x => x.Order).ToList();
        }

        public List<Menu> GetMenus()
        {
            var section = Configuration.GetSection("extensions:menus");
            return GetMenus(section);
        }

        private List<Menu> GetMenus(IConfigurationSection section)
        {
            var menus = new List<Menu>();
            if (section.Exists())
            {
                var children = section.GetChildren();
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        var menu = child.Get<Menu>();
                        var childSection = child.GetSection("menus");

                        menus.Add(new Menu
                        {
                            Id = child.Key,
                            Text = menu.Text,
                            Type = menu.Type,
                            IconClass = menu.IconClass,
                            Link = menu.Link,
                            Target = menu.Target,
                            Permissions = menu.Permissions,
                            Order = menu.Order,
                            Children = GetMenus(childSection)
                        });
                    }
                }
            }

            return menus.OrderByDescending(x => x.Order.HasValue).ThenBy(x => x.Order).ToList();
        }

        public List<Table> GetTables()
        {
            var tables = new List<Table>();
            var section = Configuration.GetSection("extensions:tables");
            if (!section.Exists()) return tables;

            var list = section.Get<Dictionary<string, Table>>();
            foreach (var (key, value) in list)
            {
                var table = value;
                table.Id = key;
                tables.Add(table);
            }

            return tables;
        }


        public List<string> GetContentTableNames()
        {
            var tables = GetTables();
            return tables
                .Where(x => StringUtils.EqualsIgnoreCase(x.Type, Types.TableTypes.Content)).Select(x => x.Id)
                .ToList();
        }
    }
}
