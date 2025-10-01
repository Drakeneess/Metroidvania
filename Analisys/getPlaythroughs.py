from modules.session_logs import get_playthroughs_with_logs

if __name__ == "__main__":
    print("=== Sesiones con Logs ===")
    sessions = get_playthroughs_with_logs(limit=5)
    for s in sessions:
        print("▶", s)
