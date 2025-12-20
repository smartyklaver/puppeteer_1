# from https://www.computervision.zone/lessons/code-files-14/
import cv2
import mediapipe as mp
import time
import math
import socket
import json

UDP_IP = "127.0.0.1"
UDP_PORT = 56001
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
addr = (UDP_IP, UDP_PORT)

class poseDetector():

    def __init__(self, mode=False, upBody=False, smooth=True,
                 detectionCon=0.5, trackCon=0.5):

        self.mode = mode
        self.upBody = upBody
        self.smooth = smooth
        self.detectionCon = float(detectionCon)
        self.trackCon = float(trackCon)

        self.mpDraw = mp.solutions.drawing_utils
        self.mpPose = mp.solutions.pose
        self.pose = self.mpPose.Pose(
            static_image_mode=self.mode,
            model_complexity=1,
            smooth_landmarks=self.smooth,
            enable_segmentation=False,
            min_detection_confidence=self.detectionCon,
            min_tracking_confidence=self.trackCon
        )

    def findPose(self, img, draw=True):
        imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        self.results = self.pose.process(imgRGB)
        if self.results.pose_landmarks and draw:
            self.mpDraw.draw_landmarks(
                img, self.results.pose_landmarks, self.mpPose.POSE_CONNECTIONS
            )
        return img

    def findPosition(self, img, draw=True):
        self.lmList = []
        if self.results and self.results.pose_landmarks:
            h, w, c = img.shape
            for id, lm in enumerate(self.results.pose_landmarks.landmark):
                cx, cy = int(lm.x * w), int(lm.y * h)
                self.lmList.append([id, cx, cy])
                if draw:
                    cv2.circle(img, (cx, cy), 5, (255, 0, 0), cv2.FILLED)
        return self.lmList

    def findAngle(self, img, p1, p2, p3, draw=True):
        # Get the landmarks
        x1, y1 = self.lmList[p1][1:]
        x2, y2 = self.lmList[p2][1:]
        x3, y3 = self.lmList[p3][1:]

        # Calculate the Angle at p2
        angle = math.degrees(
            math.atan2(y3 - y2, x3 - x2) - math.atan2(y1 - y2, x1 - x2)
        )
        if angle < 0:
            angle += 360

        if draw:
            cv2.line(img, (x1, y1), (x2, y2), (255, 255, 255), 3)
            cv2.line(img, (x3, y3), (x2, y2), (255, 255, 255), 3)
            cv2.circle(img, (x1, y1), 10, (0, 0, 255), cv2.FILLED)
            cv2.circle(img, (x1, y1), 15, (0, 0, 255), 2)
            cv2.circle(img, (x2, y2), 10, (0, 0, 255), cv2.FILLED)
            cv2.circle(img, (x2, y2), 15, (0, 0, 255), 2)
            cv2.circle(img, (x3, y3), 10, (0, 0, 255), cv2.FILLED)
            cv2.circle(img, (x3, y3), 15, (0, 0, 255), 2)
            cv2.putText(img, str(int(angle)), (x2 - 50, y2 + 50),
                        cv2.FONT_HERSHEY_PLAIN, 2, (0, 0, 255), 2)

        return angle

def inverse_lerp(a, b, v):
    return max(0.0, min(1.0, (v - a) / (b - a)))

def main():
    # cap = cv2.VideoCapture('testVideo.mp4')
    cap = cv2.VideoCapture(1)
    pTime = 0
    detector = poseDetector()
    try:
        while True:
            success, img = cap.read()
            if not success:
                print('video end')
                break  # end of video

            img = detector.findPose(img)
            lmList = detector.findPosition(img, draw=False)

            if len(lmList) != 0:
 
                angleRightArm  =  detector.findAngle(img, p1=14, p2=12, p3=24, draw=False)
                angleLeftArm =  detector.findAngle(img, p1=13, p2=11, p3=23, draw=False)
                print("left raw= ", angleLeftArm)
                print("right raw= ", angleRightArm)

              #  left_norm  =  min(1.0, max(0.0, angleLeftArm / 360.0)) 
              #  right_norm =  min(1.0, max(0.0, angleRightArm / 360.0)) 
                print("left norm= ", angleLeftArm)
                print("right norm= ", angleRightArm)
                # angleTorso = detector.findAngle(img, p1=24, p2=12, p3=0, draw=True)
                # print("torso angle:", angleTorso)

                # if angleTorso > 180:
                #  angleTorso = 360 - angleTorso

                # torso_norm = (180 - angleTorso) / 90.0
                # torso_norm = min(60, max(-30, torso_norm))

                x1, y1 = lmList[24][1:]  # RIGHT_HIP
                x2, y2 = lmList[12][1:]  # RIGHT_SHOULDER

                dx = x2 - x1
                dy = y2 - y1

                angleTorso = math.degrees(math.atan2(dy, dx))
                if angleTorso < 0:
                 angleTorso += 360

                torso_norm = inverse_lerp(280, 210, angleTorso)

                # debug
                cv2.putText(img, str(round(torso_norm, 1)), (x2 + 20, y2), cv2.FONT_HERSHEY_PLAIN, 2, (0, 0, 255), 2)


                payload = {
                    "leftShoulderValue":  round(angleLeftArm,  2),
                    "rightShoulderValue": round(angleRightArm, 2),
                    "torsoBend":          round(torso_norm, 2)
                }
                sock.sendto(json.dumps(payload).encode("utf-8"), addr)

                #time.sleep(0.1)  
                cv2.circle(img, (lmList[14][1], lmList[14][2]), 15, (0, 0, 255), cv2.FILLED)



            cTime = time.time()
            fps = 1 / (cTime - pTime) if (cTime - pTime) > 0 else 0
            pTime = cTime

            cv2.putText(img, str(int(fps)), (70, 50), cv2.FONT_HERSHEY_PLAIN, 3,
                        (255, 0, 0), 3)

            cv2.imshow("Image", img)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break
    finally:
        cap.release()
        cv2.destroyAllWindows()
        sock.close() 

if __name__ == "__main__":
    main()
