using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyStack<T> {
    private List<T> list;
    
    public MyStack() {
        list = new List<T>();
    }

    public T top() {
        return list[list.Count - 1];
    }

    public void push(T item) {
        list.Add(item);
    }

    public void pop() {
        list.RemoveAt(list.Count - 1);
    }

    public int getSize() {
        return list.Count;
    }
}
