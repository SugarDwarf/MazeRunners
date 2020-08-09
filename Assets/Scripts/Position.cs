using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Position {
    public Vector2 pos;
    public ArrayList direction = new ArrayList();//nord sud est ouest

    public Position(float x, float y) {
        pos.x = x;
        pos.y = y;
        direction.Add(false);
        direction.Add(false);
        direction.Add(false);
        direction.Add(false);
    }

    public int getNumberOfDirections() {
        int numberOfDirections = 0;
        if ((bool)direction[0])
            numberOfDirections++;
        if ((bool)direction[1])
            numberOfDirections++;
        if ((bool)direction[2])
            numberOfDirections++;
        if ((bool)direction[3])
            numberOfDirections++;
        return numberOfDirections;
    }

    public Vector2 rightEdgePos() {
        return new Vector2((float)(pos.x + 0.5), pos.y);
    }

    public Vector2 leftEdgePos() {
        return new Vector2((float)(pos.x - 0.5), pos.y);
    }

    public Vector2 upEdgePos() {
        return new Vector2(pos.x, (float)(pos.y + 0.5));
    }

    public Vector2 downEdgePos() {
        return new Vector2(pos.x, (float)(pos.y - 0.5));
    }
};
