public class Player {
    string name;
    int score;
    int connectionId;
    string move;

    public Player() {
        move = null;
    }

    public Player(int connectionId, string name, int score) {
        this.name = name;
        this.score = score;
        this.connectionId = connectionId;
        move = null;
    }
}