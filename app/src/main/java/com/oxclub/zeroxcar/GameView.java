package com.oxclub.zeroxcar;

import android.content.Context;
import android.content.SharedPreferences;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.RectF;
import android.view.MotionEvent;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.Random;

public class GameView extends SurfaceView implements Runnable {

    private Thread thread;
    private boolean running = false;
    private SurfaceHolder holder;
    private Paint paint;
    private Random random = new Random();

    private int screenW, screenH;
    private float laneW;
    private int playerLane = 1;
    private float playerX, playerY, playerW, playerH;
    private final int LANES = 3;

    private float roadOffset = 0f;
    private float baseSpeed = 12f;
    private float speed = 12f;

    private ArrayList<RectF> enemies = new ArrayList<>();
    private long lastSpawn = 0;
    private float spawnIntervalMs = 900f;
    private float score = 0f;
    private float highScore = 0f;
    private boolean gameOver = false;

    private long lastFrame = 0;
    private float deltaTime = 0f;
    private SharedPreferences prefs;

    public GameView(Context context) {
        super(context);
        holder = getHolder();
        paint = new Paint(Paint.ANTI_ALIAS_FLAG);
        prefs = context.getSharedPreferences("0xcar", Context.MODE_PRIVATE);
        highScore = prefs.getFloat("high", 0f);
    }

    @Override
    protected void onSizeChanged(int w, int h, int oldw, int oldh) {
        super.onSizeChanged(w, h, oldw, oldh);
        screenW = w; screenH = h;
        laneW = screenW / (float) LANES;
        playerW = laneW * 0.6f;
        playerH = playerW * 1.8f;
        playerY = screenH - playerH - 80;
        playerX = laneToX(playerLane);
        lastFrame = System.nanoTime();
    }

    private float laneToX(int lane) {
        return lane * laneW + (laneW - playerW) / 2f;
    }

    @Override
    public void run() {
        while (running) {
            long now = System.nanoTime();
            deltaTime = (now - lastFrame) / 1_000_000_000f;
            lastFrame = now;
            if (deltaTime > 0.05f) deltaTime = 0.05f;

            if (!holder.getSurface().isValid()) continue;
            Canvas c = holder.lockCanvas();
            if (c != null) {
                update();
                draw(c);
                holder.unlockCanvasAndPost(c);
            }
        }
    }

    private void update() {
        if (gameOver) return;
        speed = baseSpeed + score * 0.02f;
        roadOffset += speed * deltaTime * 60f;
        if (roadOffset > 120f) roadOffset -= 120f;
        score += speed * deltaTime * 2f;

        float targetX = laneToX(playerLane);
        playerX += (targetX - playerX) * Math.min(1f, deltaTime * 12f);

        long now = System.currentTimeMillis();
        float interval = Math.max(350f, spawnIntervalMs - score * 0.15f);
        if (now - lastSpawn > interval) { spawnEnemy(); lastSpawn = now; }

        float enemySpeed = speed * 60f;
        RectF playerRect = new RectF(playerX, playerY, playerX + playerW, playerY + playerH);

        Iterator<RectF> it = enemies.iterator();
        while (it.hasNext()) {
            RectF e = it.next();
            e.top += enemySpeed * deltaTime;
            e.bottom += enemySpeed * deltaTime;
            if (e.top > screenH) { it.remove(); continue; }
            if (RectF.intersects(e, playerRect)) {
                gameOver = true;
                if (score > highScore) {
                    highScore = score;
                    prefs.edit().putFloat("high", highScore).apply();
                }
            }
        }
    }

    private void spawnEnemy() {
        int lane;
        do { lane = random.nextInt(LANES); }
        while (lane == playerLane && random.nextFloat() < 0.7f);

        float x = lane * laneW + (laneW - playerW) / 2f;
        float y = -playerH - 20;
        enemies.add(new RectF(x, y, x + playerW, y + playerH));
    }

    private void draw(Canvas c) {
        c.drawColor(Color.parseColor("#1a1a1a"));

        paint.setColor(Color.parseColor("#444444"));
        paint.setStrokeWidth(6f);
        for (int i = 1; i < LANES; i++) {
            float x = i * laneW;
            for (float y = -roadOffset; y < screenH; y += 120f) {
                c.drawLine(x, y, x, y + 60f, paint);
            }
        }

        paint.setColor(Color.parseColor("#ffcc00"));
        c.drawRect(0, 0, 8, screenH, paint);
        c.drawRect(screenW - 8, 0, screenW, screenH, paint);

        for (RectF e : enemies) {
            paint.setColor(Color.parseColor("#e74c3c"));
            c.drawRoundRect(e, 12, 12, paint);
            paint.setColor(Color.WHITE);
            c.drawRect(e.left + e.width() * 0.15f, e.top + e.height() * 0.1f,
                    e.right - e.width() * 0.15f, e.top + e.height() * 0.28f, paint);
            c.drawRect(e.left + e.width() * 0.15f, e.bottom - e.height() * 0.28f,
                    e.right - e.width() * 0.15f, e.bottom - e.height() * 0.1f, paint);
        }

        RectF pr = new RectF(playerX, playerY, playerX + playerW, playerY + playerH);
        paint.setColor(Color.parseColor("#3498db"));
        c.drawRoundRect(pr, 12, 12, paint);
        paint.setColor(Color.WHITE);
        c.drawRect(pr.left + pr.width() * 0.15f, pr.top + pr.height() * 0.1f,
                pr.right - pr.width() * 0.15f, pr.top + pr.height() * 0.28f, paint);
        c.drawRect(pr.left + pr.width() * 0.15f, pr.bottom - pr.height() * 0.28f,
                pr.right - pr.width() * 0.15f, pr.bottom - pr.height() * 0.1f, paint);

        paint.setColor(Color.WHITE);
        paint.setTextSize(48f);
        paint.setTextAlign(Paint.Align.LEFT);
        c.drawText("Score: " + (int) score, 30, 70, paint);
        paint.setTextAlign(Paint.Align.RIGHT);
        c.drawText("Best: " + (int) highScore, screenW - 30, 70, paint);
        paint.setTextAlign(Paint.Align.LEFT);
        paint.setTextSize(36f);
        c.drawText("Speed: " + (int) (speed * 10) + " km/h", 30, 120, paint);

        if (gameOver) {
            paint.setColor(Color.parseColor("#CC000000"));
            c.drawRect(0, 0, screenW, screenH, paint);
            paint.setColor(Color.WHITE);
            paint.setTextAlign(Paint.Align.CENTER);
            paint.setTextSize(90f);
            c.drawText("GAME OVER", screenW / 2f, screenH / 2f - 80, paint);
            paint.setTextSize(54f);
            c.drawText("Score: " + (int) score, screenW / 2f, screenH / 2f + 10, paint);
            c.drawText("Best: " + (int) highScore, screenW / 2f, screenH / 2f + 80, paint);
            paint.setTextSize(44f);
            paint.setColor(Color.parseColor("#00ff88"));
            c.drawText("TAP TO RESTART", screenW / 2f, screenH / 2f + 180, paint);
        }
    }

    @Override
    public boolean onTouchEvent(MotionEvent event) {
        if (event.getAction() == MotionEvent.ACTION_DOWN) {
            if (gameOver) { resetGame(); return true; }
            float x = event.getX();
            if (x < screenW / 2f) { if (playerLane > 0) playerLane--; }
            else { if (playerLane < LANES - 1) playerLane++; }
            return true;
        }
        return super.onTouchEvent(event);
    }

    private void resetGame() {
        enemies.clear();
        score = 0f;
        speed = baseSpeed;
        playerLane = 1;
        playerX = laneToX(playerLane);
        gameOver = false;
        lastSpawn = System.currentTimeMillis();
    }

    public void resume() { running = true; thread = new Thread(this); thread.start(); }

    public void pause() {
        running = false;
        try { if (thread != null) thread.join(); } catch (InterruptedException ignored) {}
    }
}
