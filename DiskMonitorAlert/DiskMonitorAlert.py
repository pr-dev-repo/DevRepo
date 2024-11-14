import smtplib
import psutil
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText

# Configuration
SMTP_SERVER = "smtp.gmail.com"
SMTP_PORT = 587
SENDER_EMAIL = "your_email@gmail.com"
SENDER_PASSWORD = "your_password"  # Use an app password if 2FA is enabled
RECIPIENT_EMAIL = "recipient_email@gmail.com"
THRESHOLD = 90  # Threshold percentage for disk usage

def check_disk_usage():
    # Check the C: drive disk usage
    usage = psutil.disk_usage("C:\\")
    print(f"C: Drive Usage: {usage.percent}%")
    return usage.percent

def send_alert_email(disk_usage):
    # Set up the MIME message
    message = MIMEMultipart()
    message["From"] = SENDER_EMAIL
    message["To"] = RECIPIENT_EMAIL
    message["Subject"] = "Disk Space Alert: C: Drive Over 90% Capacity"

    # Email body
    body = f"Warning: The C: drive is at {disk_usage}% capacity. Please take action to free up space."
    message.attach(MIMEText(body, "plain"))

    # Send the email via SMTP
    try:
        with smtplib.SMTP(SMTP_SERVER, SMTP_PORT) as server:
            server.starttls()  # Secure the connection
            server.login(SENDER_EMAIL, SENDER_PASSWORD)
            server.sendmail(SENDER_EMAIL, RECIPIENT_EMAIL, message.as_string())
            print("Alert email sent successfully.")
    except Exception as e:
        print(f"Failed to send email: {e}")

def main():
    disk_usage = check_disk_usage()
    print(f"C: Drive Usage: {disk_usage}%")
    
    if disk_usage > THRESHOLD:
        send_alert_email(disk_usage)
    else:
        print("Disk usage is within acceptable limits.")

if __name__ == "__main__":
    main()