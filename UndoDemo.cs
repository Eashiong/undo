using System;
using System.Collections.Generic;
using Undo;
namespace Undo.Demo
{

    public class UndoDemo
    {
        //Create an ChangeStack to store changes
        private const uint LIMIT = 5;
        private readonly ChangeStack<string> changes = new ChangeStack<string>(LIMIT);

        //按数据方式
        private void Fun()
        {
            Person person = new Person("");

            //Add new undo, redo commands using ChangeStack.add(). When a change is added, it calls the change's execute() method. Use Change() for simple inline changes.
            changes.Add(new Change<string>(
                oldValue: person.Name,
                execute: () => person.Name = "Jane",
                undo: (oldValue) => person.Name = oldValue
                ));

            //Undo a change with undo().
            //print(person.firstName); // Jane
            changes.Undo();
            //print(person.firstName); // John

            //Redo the change with redo().
            changes.Redo();
            //print(person.firstName); // Jane

        }

        //按方法方式
        private readonly ChangeStack<Action> changes2 = new ChangeStack<Action>(LIMIT);
        private void Fun2()
        {
            Team team = new Team();
            Person person = new Person("Jane");

            changes2.Add(new Change<Action>(
                oldValue: ()=>
                {
                    team.Remove(person);
                },
                execute: () =>
                {
                    team.Add(person);
                },
                undo: (oldValue) => oldValue()
                ));

            //print(team.people.Count); // 1

            //Undo a change with undo().
            changes2.Undo();
            //print(team.people.Count); // 0

            //Redo the change with redo().
            changes.Redo();
            //print(team.people.Count); // 1

        }


    }

    public class Person
    {
        public string Name { get; set; }
        public Person(string name) => this.Name = name;

    }

    public class Team
    {
        public readonly List<Person> people = new List<Person>();
        public void Add(Person person)
        {
            people.Add(person);
        }
        public void Remove(Person person)
        {
            people.Remove(person);
        }
    }
}
