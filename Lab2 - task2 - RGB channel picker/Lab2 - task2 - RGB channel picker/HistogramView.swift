import Cocoa
import CoreGraphics

class HistogramViewController: NSViewController {
    @IBOutlet var redHistogram: NSImageView!
    @IBOutlet var greenHistogram: NSImageView!
    @IBOutlet var blueHistogram: NSImageView!
    
    var histogramValues: (red: [Int], green: [Int], blue: [Int])?
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
        
        drawHistogram()
    }
    
    @IBAction func Close(_ sender: Any) {
        dismiss(self)
    }
    
    func drawHistogram() {
        let colorSpace = CGColorSpaceCreateDeviceRGB()
        let ctx = CGContext(data: nil, width: Int(redHistogram.frame.width), height: Int(redHistogram.frame.height), bitsPerComponent: 8, bytesPerRow: 0, space: colorSpace, bitmapInfo: CGImageAlphaInfo.noneSkipFirst.rawValue)
        
        if let context = ctx {
            let pathRed = CGMutablePath()
            let pathGreen = CGMutablePath()
            let pathBlue = CGMutablePath()
            
            let redMax = CGFloat((histogramValues?.red.max()!)!)
            let greenMax = CGFloat((histogramValues?.red.max()!)!)
            let blueMax = CGFloat((histogramValues?.red.max()!)!)
            
            var point = CGPoint(x: 0, y: redHistogram.frame.size.height)
            let xScale = CGFloat(histogramValues!.red.count) / redHistogram.frame.size.width
            for i in 0...histogramValues!.red.count - 1 {
                pathRed.addRect(CGRect(origin: point,
                                       size: CGSize(width: 1 / xScale,
                                                    height: -CGFloat(histogramValues!.red[i]) * redHistogram.frame.size.height / redMax)))
                pathGreen.addRect(CGRect(origin: point,
                                         size: CGSize(width: 1 / xScale,
                                                      height: -CGFloat(histogramValues!.green[i]) * greenHistogram.frame.size.height / greenMax)))
                pathBlue.addRect(CGRect(origin: point,
                                        size: CGSize(width: 1 / xScale,
                                                     height: -CGFloat(histogramValues!.blue[i]) * blueHistogram.frame.size.height / blueMax)))
                point.x += 1 / xScale
            }
            
            context.setLineWidth(1.0)
            
            context.setStrokeColor(CGColor(red: 255, green: 0, blue: 0, alpha: 255))
            context.setFillColor(CGColor(red: 255, green: 0, blue: 0, alpha: 1))
            context.addPath(pathRed)
            context.drawPath(using: .fillStroke)
            redHistogram.image = NSImage(cgImage: context.makeImage()!, size: redHistogram.frame.size)
            redHistogram.rotate(byDegrees: CGFloat(180.0))
            
            context.closePath()
            context.setStrokeColor(CGColor(red: 0, green: 255, blue: 0, alpha: 255))
            context.setFillColor(CGColor(red: 0, green: 255, blue: 0, alpha: 255))
            context.clear(redHistogram.frame)
            context.addPath(pathGreen)
            context.drawPath(using: .fillStroke)
            greenHistogram.image = NSImage(cgImage: context.makeImage()!, size: greenHistogram.frame.size)
            greenHistogram.rotate(byDegrees: CGFloat(180.0))
            
            context.setStrokeColor(CGColor(red: 0, green: 0, blue: 255, alpha: 255))
            context.setFillColor(CGColor(red: 0, green: 0, blue: 255, alpha: 255))
            context.clear(redHistogram.frame)
            context.addPath(pathBlue)
            context.drawPath(using: .fillStroke)
            blueHistogram.image = NSImage(cgImage: context.makeImage()!, size: blueHistogram.frame.size)
            blueHistogram.rotate(byDegrees: CGFloat(180.0))
        }
    }
}
