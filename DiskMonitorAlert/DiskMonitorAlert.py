import os
import psutil
import base64
from google.auth.transport.requests import Request
from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
from email.mime.text import MIMEText

# OAuth 2.0 setup
SCOPES = ['https://www.googleapis.com/auth/gmail.send']
CREDS_PATH = r'C:\Users\Gabrirodriguez\Documents\DevRepo\DiskMonitorAlert\credentials.json'
TOKEN_PATH = 'token.json'  # Path to store token
DISK_THRESHOLD = 90

def get_creds():
    creds = None
    if os.path.exists(TOKEN_PATH):
        creds = Credentials.from_authorized_user_file(TOKEN_PATH, SCOPES)
    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            flow = InstalledAppFlow.from_client_secrets_file(CREDS_PATH, SCOPES)
            creds = flow.run_local_server(port=0)
        with open(TOKEN_PATH, 'w') as token:
            token.write(creds.to_json())
    return creds

def send_alert_email(creds, disk_usage):
    try:
        service = build('gmail', 'v1', credentials=creds)
        message = MIMEText(f"Warning: The C: drive is at {disk_usage}% capacity.")
        message['to'] = "g-rodz@hotmail.com"
        message['from'] = "cdle.dms.services@gmail.com"
        message['subject'] = "Disk Space Alert: C: Drive Over 90% Capacity"
        raw = base64.urlsafe_b64encode(message.as_bytes()).decode()
        message_body = {'raw': raw}
        service.users().messages().send(userId='me', body=message_body).execute()
        print("Alert email sent successfully.")
    except HttpError as error:
        print(f"An error occurred: {error}")

def check_disk_usage():
    usage = psutil.disk_usage("C:\\")
    print(f"C: Drive Usage: {usage.percent}%")
    return usage.percent

def main():
    disk_usage = check_disk_usage()
    print(f"C: Drive Usage: {disk_usage}%")
    if disk_usage > DISK_THRESHOLD:
        creds = get_creds()
        send_alert_email(creds, disk_usage)
    else:
        print("Disk usage is within acceptable limits.")

if __name__ == "__main__":
    main()